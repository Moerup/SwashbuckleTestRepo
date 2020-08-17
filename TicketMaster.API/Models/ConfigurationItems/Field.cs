using TicketMaster.API.Interfaces;

namespace TicketMaster.API.Models
{
    public class Field : IBaseModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
