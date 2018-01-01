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
        private const string ContainerFormKey = "Container";

        private const string ResourceKeyFormKey = "ResourceKey";

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

            var metadata = new Dictionary<string, string>();
            string container = string.Empty;
            string fileKey = string.Empty;
            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        metadata.Add("Content-Type", section.ContentType);
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).ToString();
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }

                            switch(key)
                            {
                                case ContainerFormKey:
                                    container = value;
                                    break;
                                case ResourceKeyFormKey:
                                    fileKey = value;
                                    break;
                                default:
                                    if (key.StartsWith("X-"))
                                        metadata.Add(key, value);
                                    break;
                            }
                        }
                    }
                }                

                section = await reader.ReadNextSectionAsync();
            }

            if (string.IsNullOrWhiteSpace(container))
                return BadRequestMsg($"Form data does not contain '{ContainerFormKey}' key.");

            if (string.IsNullOrWhiteSpace(fileKey))
                return BadRequestMsg($"Form data does not contain '{ResourceKeyFormKey}' key.");
            
            await _service.SaveAsync();

            return Json(new
            {
                RequestId = HttpContext.TraceIdentifier,
                ServerTimeUTC = DateTime.UtcNow.ToString("o")
            });
        }

        private IActionResult BadRequestMsg(string msg)
        {
            return BadRequest(new ErrorResponse(HttpContext.TraceIdentifier, msg));            
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }               
    }
}