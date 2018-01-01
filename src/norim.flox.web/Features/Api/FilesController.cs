using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using norim.flox.web.Attributes;
using norim.flox.web.Models;
using norim.flox.web.Services;
using norim.flox.web.Utilities;

namespace norim.flox.web.Features.Api
{
    public class FilesController : Controller
    {
        private readonly IFileService _service;

        public FilesController(IFileService service)
        {
            _service = service;
        }

        [HttpPost, DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest(new ErrorResponse(HttpContext.TraceIdentifier, $"Expected a multipart request, but got {Request.ContentType}"));
            }            
            
            await _service.SaveAsync();

            return Json(new
            {
                RequestId = HttpContext.TraceIdentifier,
                ServerTimeUTC = DateTime.UtcNow.ToString("o")
            });
        }       
    }
}