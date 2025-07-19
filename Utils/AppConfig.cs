using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        public static Dictionary<string, string> LoadKeywordMapping()
        {
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "drinkKeywords.json");
                var json = File.ReadAllText(jsonPath);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Logger.Error("DrinkLoader", "Failed to load drink keyword mappings", ex);
                return new Dictionary<string, string>();
            }
        }
    }
}
