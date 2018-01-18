using System;
using MongoDB.Bson.Serialization.Attributes;

namespace norim.flox.domain.Models
{
    public class FloxContainer
    {
        [BsonId]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}