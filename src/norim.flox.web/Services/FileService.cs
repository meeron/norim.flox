using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using norim.flox.core;
using norim.flox.domain;
using norim.flox.web.Models;
using norim.flox.web.Utilities;

namespace norim.flox.web.Services
{
    public class FileService : IFileService
    {        
        private readonly IFileRepository _repository;

        public FileService(IFileRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveAsync(string boundary, Stream bodyStream)
        {
            MultipartSection section = null;
            var formData = new NameValueCollection();

            var reader = new MultipartReader(boundary, bodyStream);

            using(var tempFile = TempFile.Create())
            {
                section = await reader.ReadNextSectionAsync();
                while(section != null)
                {
                    ContentDispositionHeaderValue contentDisposition = null;
                    if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition))
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            if (formData["LocalPath"] != null)
                                throw new Exception("Only one file at the time is allowed.");
                            
                            using(var fs = File.Create(tempFile.Path))
                            {
                                await section.Body.CopyToAsync(fs);
                            }

                            formData.Add("LocalPath", tempFile.Path);
                            formData.Add("Content-Type", section.ContentType);
                        }
                        else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                        {
                            // Content-Disposition: form-data; name="key"
                            //
                            // value

                            // Do not limit the key name length here because the 
                            // multipart headers length limit is already in effect.
                            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).ToString();
                            var value = await GetValue(section.Body, GetEncoding(section));
                            
                            formData.Add(key, value);
                        }
                    }

                    section = await reader.ReadNextSectionAsync();
                }
                
                var fileToSave = FileToSave.Map(formData);
                fileToSave.DeleteLocalFileAfterSave = false;

                await _repository.SaveAsync(fileToSave);
            }
        }

        private static async Task<string> GetValue(Stream bodyStream, Encoding encoding)
        {
            using (var streamReader = new StreamReader(
                bodyStream,
                encoding,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        public async Task DeleteAsync(DeleteFileRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.ThrowIfInvalid();

            await _repository.DeleteAsync(request.Container, request.ResourceKey, request.CheckFile);
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