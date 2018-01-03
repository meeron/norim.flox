using System;
using System.IO;
using SPath = System.IO.Path;

namespace norim.flox.core
{
    public class TempFile : IDisposable
    {
        private TempFile()
        {
            Path = SPath.Combine(SPath.GetTempPath(), SPath.GetRandomFileName());
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                if (File.Exists(Path))
                    File.Delete(Path);
            }
            catch (System.Exception) {}
        }

        public static TempFile Create()
        {
            return new TempFile();
        }
    }
}