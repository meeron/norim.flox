using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using norim.flox.core.Configuration;
using norim.flox.domain.Models;

namespace norim.flox.domain.Implementations
{
    public class MongoDbRepository : FileRepositoryBase
    {
        private const long FileInDocumentThreshold = 15728640;

        private readonly IMongoClient _client;
        
        public MongoDbRepository(ISettings settings)
        {
            var mongoUrl = new MongoUrl(settings.Domain.MongoDbConnectionString);

            _client = new MongoClient(mongoUrl);
        }

        protected override async Task DeleteInternalAsync(string container, string key)
        {
            var fileContainer = GetFileContainer(container);

            var item = await fileContainer.FindOneAndDeleteAsync(f => f.Key == key);
            if (item != null && !item.FileId.Equals(ObjectId.Empty))
            {
                await GetGridFSBucket(container).DeleteAsync(item.FileId);
            }
        }

        protected override bool Exists(string container, string key)
        {
            var fileContainer = GetFileContainer(container);

            return fileContainer.Find(f => f.Key == key).Any();
        }

        protected override async Task<FileData> GetInternalAsync(string container, string key, bool onlyMetadata)
        {
            var fileContainer = GetFileContainer(container);

            var floxFile = (await fileContainer.FindAsync(f => f.Key == key)).FirstOrDefault();

            if (floxFile == null)
                return null;

            if (onlyMetadata)
               return new FileData(null, floxFile.Metadata, floxFile.Length);

            if (floxFile.Content == null)
                return new FileData(GetGridFSBucket(container).OpenDownloadStream(floxFile.FileId), floxFile.Metadata);

            return new FileData(new MemoryStream(floxFile.Content), floxFile.Metadata);
        }

        protected override async Task SaveInternalAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata)
        {
            byte[] buffer = null;
            ObjectId gridFsId = ObjectId.Empty;

            var fileContainer = GetFileContainer(container);
            var currentItem = (await fileContainer.FindAsync(f => f.Key == key)).FirstOrDefault();

            if (fileStream.Length > FileInDocumentThreshold)
            {
                gridFsId = await GetGridFSBucket(container).UploadFromStreamAsync($"{container}/{key}", fileStream);
            }
            else
            {
                buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, buffer.Length);
            }

            var newItem = new FloxFile
            {
                Key = key,
                Content = buffer,
                FileId = gridFsId,
                Length = fileStream.Length,
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (currentItem != null)
            {
                var updateDefinition = Builders<FloxFile>.Update
                    .Set(f => f.Content, newItem.Content)
                    .Set(f => f.FileId, newItem.FileId)
                    .Set(f => f.Length, newItem.Length)
                    .Set(f => f.UpdatedAt, newItem.UpdatedAt)
                    .Set(f => f.Metadata, newItem.Metadata);

                await fileContainer.FindOneAndUpdateAsync(f => f.Key == key, updateDefinition);

                if (!currentItem.FileId.Equals(ObjectId.Empty))
                {
                    await GetGridFSBucket(container).DeleteAsync(currentItem.FileId);
                }
            }
            else
                await fileContainer.InsertOneAsync(newItem);
        }

        private IMongoCollection<FloxFile> GetFileContainer(string container)
        {
            var database = _client.GetDatabase($"flox_cnt_{container}");
            return database.GetCollection<FloxFile>($"flx.files");
        }

        private IGridFSBucket GetGridFSBucket(string container)
        {
            var database = _client.GetDatabase($"flox_cnt_{container}");
            return new GridFSBucket(database, new GridFSBucketOptions
            {
                ChunkSizeBytes = 1048576
            });        
        }
    }
}
