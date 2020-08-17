using TicketMaster.API.Interfaces;

namespace TicketMaster.API.Models
{
    public class Product : IBaseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
