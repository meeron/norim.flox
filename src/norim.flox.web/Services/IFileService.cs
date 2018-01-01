using System.IO;
using System.Threading.Tasks;

namespace norim.flox.web.Services
{
    public interface IFileService
    {
         Task SaveAsync();
    }
}