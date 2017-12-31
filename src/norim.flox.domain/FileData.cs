using System.IO;
using System.Collections.Generic;
using System;

namespace norim.flox.domain
{
    public class FileData: IDisposable
    {
        internal FileData(Stream stream, IDictionary<string, string> metadata)
        {
            Content = stream;
            Metadata = metadata;
        }

        public Stream Content { get; private set; }

        public IDictionary<string, string> Metadata { get; }

        public void Dispose()
        {
            Content.Dispose();
            Content = null;

            Metadata.Clear();
        }
    }
}