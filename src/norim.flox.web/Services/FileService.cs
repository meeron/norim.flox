using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using norim.flox.domain;
using norim.flox.web.Utilities;

namespace norim.flox.web.Services
{
    public class FileService : IFileService
    {
        private const string ContainerFormKey = "Container";

        private const string ResourceKeyFormKey = "ResourceKey";
        
        private readonly IFileRepository _repository;

        public FileService(IFileRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveAsync(string boundary, Stream bodyStream)
        {
            MultipartSection section = null;
            var metadata = new Dictionary<string, string>();
            var container = string.Empty;
            var resourceKey = string.Empty;
            var targetFilePath = string.Empty;

            var reader = new MultipartReader(boundary, bodyStream);

            section = await reader.ReadNextSectionAsync();
            while(section != null)
            {
                ContentDispositionHeaderValue contentDisposition = null;
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition))
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                        
                        using(var fs = File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(fs);
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
                        var value = await GetValue(section.Body, GetEncoding(section));

                        switch (key)
                        {
                            case ContainerFormKey:
                                container = value;
                                break;
                            case ResourceKeyFormKey:
                                resourceKey = value;
                                break;
                            default:
                                metadata.Add(key, value);
                                break;
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            if (string.IsNullOrWhiteSpace(targetFilePath))
                throw new Exception("Form data does not contain file.");

            if (string.IsNullOrWhiteSpace(container))
                throw new Exception($"Form data does not contain '{ContainerFormKey}' key.");

            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new Exception($"Form data does not contain '{ResourceKeyFormKey}' key.");

            using(var fs = File.OpenRead(targetFilePath))
            {
                await _repository.SaveAsync(container, resourceKey, fs, metadata);
            }

            File.Delete(targetFilePath);
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