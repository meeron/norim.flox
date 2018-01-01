using System.Linq;
using System.Collections.Generic;
using System.IO;
using norim.flox.core.Configuration;

namespace norim.flox.domain.Implementations.FileSystem
{
    public class FileRepository : FileRepositoryBase, IFileRepository
    {
        private readonly ISettings _settings;

        public FileRepository(ISettings settings)
        {
            _settings = settings;
        }

        protected override bool Exists(string container, string key)
        {
            return File.Exists(ToLocalPath(container, key));
        }

        protected override Stream Get(string container, string key, out IDictionary<string, string> metadata)
        {
            var filePath = ToLocalPath(container, key);
            
            metadata = null;

            if (!Exists(container, key))
                return null;

            metadata = GetMetadata(filePath);
            return File.OpenRead(filePath);
        }

        protected override void SaveInternal(string container, string key, Stream fileStream, IDictionary<string, string> metadata)
        {
            var filePath = ToLocalPath(container, key);

            using(var fs = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                fileStream.CopyTo(fs);
            }

            SaveMetadata(filePath, metadata);
        }

        private string ToLocalPath(string container, string key)
        {
            return Path.Combine(_settings.Domain.RepositoryPath, container, key.Replace("/", "\\"));
        }

        private static IDictionary<string, string> GetMetadata(string filePath)
        {
            var metadataFilePath = $"{filePath}.metadata";
            return File.ReadAllLines(metadataFilePath).ToDictionary(k => k.Split(':')[0], v => v.Split(':')[1]);
        }

        private static void SaveMetadata(string filePath, IDictionary<string, string> metadata)
        {
            var metadataFilePath = $"{filePath}.metadata";
            File.WriteAllLines(metadataFilePath, metadata.Select(x => $"{x.Key}:{x.Value}"));
        }
    }
}