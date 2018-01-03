using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace norim.flox.domain
{
    public interface IFileRepository
    {
        Task SaveAsync(FileToSave fileToSave);

        Task DeleteAsync(string container, string resourceKey, bool checkFile);

        FileData Get(string container, string key);
    }
}
