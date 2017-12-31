using System;
using System.IO;
using System.Collections.Generic;

namespace norim.flox.domain
{
    public interface IFileRepository
    {
        void Save(string key, Stream fileStream, IDictionary<string, string> metadata, bool overwrite = false);

        FileData Get(string key);
    }
}
