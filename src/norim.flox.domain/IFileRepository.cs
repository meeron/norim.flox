using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace norim.flox.domain
{
    public interface IFileRepository
    {
        Task SaveAsync(string container, string key, Stream fileStream, IDictionary<string, string> metadata, bool overwrite = false);

        FileData Get(string container, string key);
    }
}
