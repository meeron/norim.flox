using System.IO;
using System.Threading.Tasks;
using norim.flox.web.Models;

namespace norim.flox.web.Services
{
    public interface IFileService
    {
         Task SaveAsync(string boundary, Stream bodyStream);

         Task DeleteAsync(DeleteFileRequest request);
    }
}