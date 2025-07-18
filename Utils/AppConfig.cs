using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CAFEHOLIC.Utils
{
    internal static class AppConfig
    {
        private static IConfigurationRoot configuration;

        static AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        public static string Get(string keyPath)
        {
            return configuration[keyPath];
        }
    }
}
