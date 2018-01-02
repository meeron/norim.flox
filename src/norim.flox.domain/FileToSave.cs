using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace norim.flox.domain
{
    public class FileToSave
    {
        private FileToSave()
        {            
        }

        public string LocalPath { get; private set; }

        public string Container { get; private set; }

        public string ResourceKey { get; private set; }

        public IDictionary<string, string> Metadata { get; private set; }

        public bool Overwrite { get; private set; }

        public bool DeleteLocalFileAfterSave { get; set; } = true;

        public void ThrowIfInvalid()
        {
            if (string.IsNullOrWhiteSpace(LocalPath))
                throw new Exception("Form data doesn't contains file.");

            if (!File.Exists(LocalPath))
                throw new Exception("Local file doesn't exists.");

            if (new FileInfo(LocalPath).Length == 0)
                throw new Exception("Local file is invalid.");

            if (string.IsNullOrWhiteSpace(Container))
                throw new Exception($"Form data doesn't contains '{nameof(Container)}' key or value is empty.");

            if (string.IsNullOrWhiteSpace(ResourceKey))
                throw new Exception($"Form data doesn't contains '{nameof(ResourceKey)}' key or value is empty.");
        }

        public static FileToSave Map(NameValueCollection formData)
        {
            var model = new FileToSave();

            var properties = typeof(FileToSave).GetProperties().Where(p => p.Name != nameof(Metadata));

            foreach (var property in properties)
            {
                if (formData[property.Name] != null)
                {
                    property.SetValue(model, Convert.ChangeType(formData[property.Name], property.PropertyType));
                    formData.Remove(property.Name);
                }
            }

            model.Metadata = formData.AllKeys.ToDictionary(k => k, v => formData[v]);

            return model;
        }        
    }
}