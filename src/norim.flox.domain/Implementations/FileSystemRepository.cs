using System.Linq;
using System.Collections.Generic;
using System.IO;
using norim.flox.core.Configuration;
using System.Threading.Tasks;

namespace norim.flox.domain.Implementations
{
    public class FileSystemRepository : FileRepositoryBase
    {
        private readonly ISettings _settings;

        public FileSystemRepository(ISettings settings)
        {
            _settings = settings;
        }

        protected override bool Exists(string container, string key)
        {
            return File.Exists(ToLocalPath(container, key));
        }

        protected override async Task<FileData> GetInternalAsync(string container, string key)
        {
            var filePath = ToLocalPath(container, key);

            await Task.CompletedTask;
            
            return new FileData(File.OpenRead(filePath), GetMetadata(filePath));
        }

        protected override async Task SaveInternalAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata)
        {
            var filePath = ToLocalPath(container, key);
            
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using(var fs = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fs);
            }

            SaveMetadata(filePath, metadata);
        }

        protected override async Task DeleteInternalAsync(string container, string key)
        {
            await Task.Run(() =>
            {
                var localFile = ToLocalPath(container, key);
                if (File.Exists(localFile))
                {
                    File.Delete(localFile);
                    File.Delete($"{localFile}.metadata");
                }
            });
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