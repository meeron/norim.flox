using System.IO;
using System.Collections.Generic;
using System;

namespace norim.flox.domain
{
    public class FileData: IDisposable
    {
        internal FileData(Stream stream, IDictionary<string, string> metadata, long length = 0)
        {
            Content = stream;
            Metadata = metadata;
            Length = stream != null ? stream.Length : length;
        }

        public Stream Content { get; private set; }

        public long Length { get; }

        public IDictionary<string, string> Metadata { get; }

        public void Dispose()
        {
            if (Content != null)
            {
                Content.Dispose();
                Content = null;
            }

            Metadata.Clear();
        }
    }
}