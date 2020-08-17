using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TicketMaster.API.Interfaces;

namespace TicketMaster.API.Models
{
    public class WikiCategory : IBaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
