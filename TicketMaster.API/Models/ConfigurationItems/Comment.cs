using TicketMaster.API.Interfaces;

namespace TicketMaster.API.Models
{
    public class Comment : IBaseModel
    {
        public string Id { get; set; }

        public string ConfigurationItemId { get; set; }

        public int Creator { get; set; }

        public string Text { get; set; }
    }
}
