using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TicketMaster.API.Interfaces;
using TicketMaster.API.Services;

[assembly: FunctionsStartup(typeof(TicketMaster.API.Startup))]
namespace TicketMaster.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        }
    }
}