using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using norim.flox.web.Attributes;
using norim.flox.web.Models;
using norim.flox.web.Services;
using norim.flox.web.Utilities;

namespace norim.flox.web.Features.Api
{
    public class FilesController : BaseController
    {

        private readonly IFileService _service;

        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        public FilesController(IFileService service)
        {
            _service = service;
        }

        [HttpPost, DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequestObject($"Expected a multipart request, but got {Request.ContentType}");
            }

            try
            {
                var boundary = MultipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(Request.ContentType),
                    DefaultFormOptions.MultipartBoundaryLengthLimit);

                await _service.SaveAsync(boundary, Request.Body);

                return OkObject();                
            }
            catch (Exception ex)
            {
                return BadRequestObject(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteFileRequest model)
        {
            if (model == null)
                return BadRequestObject("Invalid request model.");

            try
            {
                await _service.DeleteAsync(model);

                return OkObject();
            }
            catch (System.Exception ex)
            {
                return BadRequestObject(ex.Message);
            }
        }
    }
}