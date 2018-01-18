using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;
using norim.flox.core.Configuration;
using norim.flox.domain.Models;

namespace norim.flox.domain.Implementations
{
    public class MongoDbRepository : FileRepositoryBase
    {
        private const long FileInDocumentThreshold = 15728640;

        private readonly IMongoDatabase _database;
        
        public MongoDbRepository(ISettings settings)
        {
            var mongoUrl = new MongoUrl(settings.Domain.MongoDbConnectionString);

            _database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
        }

        protected override async Task DeleteInternalAsync(string container, string key)
        {
            var fileContainer = _database.GetCollection<FloxFile>($"cnt_{container}");

            await fileContainer.FindOneAndDeleteAsync(f => f.Key == key);
        }

        protected override bool Exists(string container, string key)
        {
            var fileContainer = _database.GetCollection<FloxFile>($"cnt_{container}");

            return fileContainer.Find(f => f.Key == key).Any();
        }

        protected override async Task<FileData> GetInternalAsync(string container, string key)
        {
            var fileContainer = _database.GetCollection<FloxFile>($"cnt_{container}");

            var floxFile = (await fileContainer.FindAsync(f => f.Key == key)).FirstOrDefault();

            if (floxFile == null)
                return null;

            if (floxFile.Content == null)
                throw new NotSupportedException();

            return new FileData(new MemoryStream(floxFile.Content), floxFile.Metadata);
        }

        protected override async Task SaveInternalAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata)
        {
            if (fileStream.Length > FileInDocumentThreshold)
                throw new NotSupportedException();

            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);

            var fileContainer = _database.GetCollection<FloxFile>($"cnt_{container}");
            await fileContainer.InsertOneAsync(new FloxFile
            {
                Key = key,
                Content = buffer,
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });     
        }
    }
}