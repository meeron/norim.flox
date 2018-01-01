using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using norim.flox.web.Services;

namespace norim.flox.web.Features.Api
{
    public class FilesController : Controller
    {
        private readonly IFileService _service;

        public FilesController(IFileService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<object> Upload()
        {
            await _service.SaveAsync();

            return new
            {
                RequestId = HttpContext.TraceIdentifier,
                ServerTimeUTC = DateTime.UtcNow.ToString("o")
            };
        }       
    }
}