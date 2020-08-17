using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using TicketMaster.API.Interfaces;
using TicketMaster.API.Enums;
using TicketMaster.API.Models.UserItems;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace TicketMaster.API.Services
{

    public class AuthenticationService : IAuthenticationService
    {

        public AzureUser AuthUser { get; private set; }

        private readonly HttpClient client = new HttpClient();
        private async Task<TokenStatus> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                SecurityToken jst = tokenHandler.ReadToken(token);
                JsonWebKeySet jwks = await GetKeySet();

                var validationParameters = new TokenValidationParameters
                {
                    IssuerSigningKeys = jwks.GetSigningKeys(),
                    ValidAudience = "", // Your API Audience, can be disabled via ValidateAudience = false
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                return (validatedToken != null ? TokenStatus.Success : TokenStatus.Failed);
            }
            catch (SecurityTokenExpiredException stee)
            {
                Console.WriteLine(stee.Message);
                Console.WriteLine(stee.GetType());
                return TokenStatus.Lifetime;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.GetType());
                return TokenStatus.Failed;
            }
        }

        private async Task<JsonWebKeySet> GetKeySet()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://login.microsoftonline.com/common/discovery/v2.0/keys?appid=" + Environment.GetEnvironmentVariable("ClientId"));
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
                return JsonWebKeySet.Create(responseBody);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                throw e;
            }
        }

        private async Task<bool> userValidation(string token)
        {
            string nameClaimValue = "";
            string emailClaimValue = "";
            string subIdClaimValue = "";
            TokenStatus resToken = await ValidateToken(token);
            var securityTokenHandler = new JwtSecurityTokenHandler();
            if (securityTokenHandler.CanReadToken(token))
            {
                var decriptedToken = securityTokenHandler.ReadJwtToken(token);
                var claims = decriptedToken.Claims;
                //At this point you can get the claims in the token, in the example I am getting the expiration date claims
                //this step depends of the claims included at the moment of the token is generated
                //and what you are trying to accomplish
                emailClaimValue = claims.Where(c => c.Type == "email").FirstOrDefault().Value;
                nameClaimValue = claims.Where(c => c.Type == "name").FirstOrDefault().Value;
                subIdClaimValue = claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            }
            AzureUser aUser = new AzureUser(resToken, subIdClaimValue, nameClaimValue, emailClaimValue );
            AuthUser = aUser;

            return aUser.AuthStatus == TokenStatus.Success;
        }

        public async Task<bool> Authenticate(HttpRequest request)
        {
            StringValues keys;
            if (request.Headers.TryGetValue("token", out keys))
            {
                string token = request.Headers["token"];
                return await userValidation(token);
            }
            return false;
        }
    }
}
