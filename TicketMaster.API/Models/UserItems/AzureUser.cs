using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using TicketMaster.API.Enums;

namespace TicketMaster.API.Models.UserItems
{
    public class AzureUser
    {
        public AzureUser(TokenStatus authstatus, string subid = null, string name = null, string username = null)
        {
            SubId = subid;
            Name = name;
            Username = username;
            AuthStatus = authstatus;
            
        }
        public string? SubId { get; set; }
        public string? Name { get; private set; }
        public string? Username { get; private set; }
        public TokenStatus AuthStatus { get; private set; }
    }
}
