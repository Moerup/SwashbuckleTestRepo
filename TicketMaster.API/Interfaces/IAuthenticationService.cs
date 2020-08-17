using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using TicketMaster.API.Enums;
using TicketMaster.API.Models.UserItems;

namespace TicketMaster.API.Interfaces
{
    public interface IAuthenticationService
    {
        public AzureUser AuthUser { get; }
        public Task<bool> Authenticate(HttpRequest request);
    }
}
