using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace norim.flox.domain
{
    public abstract class FileRepositoryBase : IFileRepository
    {
        public async Task<FileData> GetAsync(string container, string key, bool onlyMetadata = false)
        {
            if (string.IsNullOrWhiteSpace(container))
                throw new ArgumentNullException(nameof(container));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return await GetInternalAsync(container, key, onlyMetadata);
        }

        public async Task SaveAsync(string container, FileToSave fileToSave)
        {
            if (fileToSave == null)
                throw new ArgumentNullException(nameof(fileToSave));

            if (string.IsNullOrWhiteSpace(container))
                throw new ArgumentNullException(nameof(container));

            fileToSave.ThrowIfInvalid();

            if (Exists(container, fileToSave.ResourceKey) && !fileToSave.Overwrite)
                throw new InvalidOperationException($"Container '{container}' already contains resource '{fileToSave.ResourceKey}'.");

            using(var fs = File.OpenRead(fileToSave.LocalPath))
            {
                await SaveInternalAsync(container, fileToSave.ResourceKey, fs, fileToSave.Metadata);
            }

            if (fileToSave.DeleteLocalFileAfterSave)
                File.Delete(fileToSave.LocalPath);
        }

        public async Task DeleteAsync(string container, string resourceKey, bool checkFile)
        {
            if (checkFile && !Exists(container, resourceKey))
                throw new Exception($"Container '{container}' doesn't contains resource '{resourceKey}'.");

            await DeleteInternalAsync(container, resourceKey);
        }

        protected abstract Task<FileData> GetInternalAsync(string container, string key, bool onlyMetadata);

        protected abstract bool Exists(string container, string key);
        
        protected abstract Task SaveInternalAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata);

        protected abstract Task DeleteInternalAsync(string container, string key);
    }
}