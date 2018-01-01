using System.Threading.Tasks;

namespace norim.flox.web.Services
{
    public class FileService : IFileService
    {
        public async Task SaveAsync()
        {
            await Task.FromResult(0);
        }
    }
}