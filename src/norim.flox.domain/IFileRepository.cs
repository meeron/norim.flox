using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace norim.flox.domain
{
    public interface IFileRepository
    {
        Task SaveAsync(string container, FileToSave fileToSave);

        Task DeleteAsync(string container, string resourceKey, bool checkFile);

        Task<FileData> GetAsync(string container, string key, bool onlyMetadata = false);
    }
}
