using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace norim.flox.domain
{
    public abstract class FileRepositoryBase : IFileRepository
    {
        public FileData Get(string container, string key)
        {
            if (string.IsNullOrWhiteSpace(container))
                throw new ArgumentNullException(nameof(container));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var stream = Get(container, key, out IDictionary<string, string> metadata);

            if (stream == null)
                return null;

            return new FileData(stream, metadata);
        }

        public async Task SaveAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(container))
                throw new ArgumentNullException(nameof(container));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (Exists(container, key) && !overwrite)
                throw new InvalidOperationException($"Container '{container}' already contains object '{key}'.");

            await SaveInternalAsync(container, key, fileStream, metadata);
        }

        protected abstract Stream Get(string container, string key, out IDictionary<string, string> metadata);

        protected abstract bool Exists(string container, string key);
        
        protected abstract Task SaveInternalAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata);
    }
}