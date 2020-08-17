using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using TicketMaster.API.Interfaces;

namespace TicketMaster.API.Models
{
    public class WikiItem : IBaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string BodyText { get; set; }
        public bool IsInternal { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string AuthorId { get; set; }
        public List<string> WikiCategoryIds { get; set; }
        public List<string> WikiTagIds { get; set; }
        public List<string> UpdatedByUsers { get; set; }
    }
}
