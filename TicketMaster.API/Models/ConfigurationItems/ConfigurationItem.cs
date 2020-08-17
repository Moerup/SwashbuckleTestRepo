using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using TicketMaster.API.Enums;
using TicketMaster.API.Interfaces;

namespace TicketMaster.API.Models
{
    public class ConfigurationItem : IBaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public ItemType ItemType { get; set; }

        public List<string> ProductIds { get; set; }

        public ConfigurationItem ConfigurationItemHistory { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public bool Active { get; set; }

        public int Creator { get; set; }

        public int Assignee { get; set; }

        public string Titel { get; set; }

        public string Description { get; set; }

        public List<string> FieldIds { get; set; }
    }
}
