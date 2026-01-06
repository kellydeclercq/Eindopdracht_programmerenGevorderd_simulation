using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimulatorBL.Configuration
{
    public class AppConfig
    {
        //initiate Appconfig so it doesn't become static class and properties can be used and filled in. (works similar to singleton)
        private static AppConfig _instance;
        public static AppConfig Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadConfiguration();
                }
                return _instance;
            }
        }

        //properties JSON file
        public ConnectionStringsConfig ConnectionStrings { get; set; }
        public SettingsConfig Settings { get; set; }

        private static AppConfig LoadConfiguration()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(basePath, "Configuration", "appsettings.json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"can't find configurationfile: {filePath}");
            }
            //read file and deserialize information
            string jsonString = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var config = JsonSerializer.Deserialize<AppConfig>(jsonString, options);

            if (config == null || config.Settings == null) //check if everything is loaded correctly
            {
                throw new Exception("JSON found, but section settings is not found.");
            }

            return config;
        }
    }
}
