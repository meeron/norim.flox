using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace norim.flox.domain
{
    public interface IFileRepository
    {
        Task SaveAsync(FileToSave fileToSave);

        FileData Get(string container, string key);
    }
}
