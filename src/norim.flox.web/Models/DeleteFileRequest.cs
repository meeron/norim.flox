using System;

namespace norim.flox.web.Models
{
    public class DeleteFileRequest
    {
        public string Container { get; set; }

        public string ResourceKey { get; set; }

        public bool CheckFile { get; set; } = true;

        public void ThrowIfInvalid()
        {
            if (string.IsNullOrWhiteSpace(Container))
                throw new Exception($"Form data doesn't contains '{nameof(Container)}' key or value is empty.");

            if (string.IsNullOrWhiteSpace(ResourceKey))
                throw new Exception($"Form data doesn't contains '{nameof(ResourceKey)}' key or value is empty.");
        }
    }
}