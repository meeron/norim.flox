using System;
using Microsoft.Extensions.Configuration;

namespace norim.flox.core.Configuration
{
    public class Settings : ISettings
    {
        public Settings(IConfiguration configuration)
        {
            Domain = configuration.GetSection("Domain").Get<DomainSettings>();

            if (Domain == null)
                throw new Exception("'Domain' section was not found in configuration file.'");
        }

        public DomainSettings Domain { get; }
    }
}