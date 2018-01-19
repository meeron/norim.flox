using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace norim.flox.domain.Models
{
    public class FloxFile
    {
        [BsonId]
        public string Key { get; set; }

        public byte[] Content { get; set;}

        public long Length { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}