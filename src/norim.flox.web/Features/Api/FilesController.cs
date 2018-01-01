using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using norim.flox.web.Attributes;
using norim.flox.web.Models;
using norim.flox.web.Services;
using norim.flox.web.Utilities;

namespace norim.flox.web.Features.Api
{
    public class FilesController : Controller
    {

        private readonly IFileService _service;

        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public FilesController(IFileService service)
        {
            _service = service;
        }

        [HttpPost, DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequestMsg($"Expected a multipart request, but got {Request.ContentType}");
            }

            try
            {
                var boundary = MultipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(Request.ContentType),
                    _defaultFormOptions.MultipartBoundaryLengthLimit);

                await _service.SaveAsync(boundary, Request.Body);

                return Json(new
                {
                    RequestId = HttpContext.TraceIdentifier,
                    ServerTimeUTC = DateTime.UtcNow.ToString("o")
                });                
            }
            catch (Exception ex)
            {
                return BadRequestMsg(ex.Message);
            }
        }

        private IActionResult BadRequestMsg(string msg)
        {
            return BadRequest(new ErrorResponse(HttpContext.TraceIdentifier, msg));            
        }
    }
}