using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace norim.flox.domain.Models
{
    public class FloxFile
    {
        [BsonId]
        public string Key { get; set; }

        [BsonElement("data")]
        public byte[] Content { get; set; }

        [BsonElement("fid")]
        public ObjectId FileId { get; set; }

        [BsonElement("len")]
        public long Length { get; set; }

        [BsonElement("meta")]
        public IDictionary<string, string> Metadata { get; set; }

        [BsonElement("crt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("upd")]
        public DateTime UpdatedAt { get; set; }
    }
}