using System.IO;
using System.Threading.Tasks;
using norim.flox.web.Models;

namespace norim.flox.web.Services
{
    public interface IFileService
    {
         Task SaveAsync(string container, string boundary, Stream bodyStream);

         Task DeleteAsync(string container, string resourceKey, bool checkFile);
    }
}