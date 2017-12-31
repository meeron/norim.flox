using System;
using System.Collections.Generic;
using System.IO;

namespace norim.flox.domain
{
    public abstract class FileRepositoryBase : IFileRepository
    {
        public FileData Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            Get(out Stream stream, out IDictionary<string, string> metadata);

            if (stream == null)
                return null;

            return new FileData(stream, metadata);
        }

        public void Save(string key, Stream fileStream, IDictionary<string, string> metadata, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (Exists(key) && !overwrite)
                throw new InvalidOperationException($"File '{key}' already exists.");

            SaveInternal(key, fileStream, metadata);
        }

        protected abstract void Get(out Stream stream, out IDictionary<string, string> metadata);

        protected abstract bool Exists(string key);
        
        protected abstract void SaveInternal(string key, Stream fileStream, IDictionary<string, string> metadata);
    }
}