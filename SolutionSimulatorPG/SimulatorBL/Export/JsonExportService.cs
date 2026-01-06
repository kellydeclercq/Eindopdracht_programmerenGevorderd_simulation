using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Export
{
    public class JsonExportService : IExportService
    {
        public bool Export(SimulationInformation stats, ExportConfiguration config, List<Customer>? customers)
        {
            // validation
            if (stats == null) throw new ExportException($"{nameof(stats)} is NULL");
            if (string.IsNullOrWhiteSpace(config.Path)) throw new ExportException("Export path can't be null or whitespace.");

            string targetDirectory;
            string fileNameNoExt;

            if (Directory.Exists(config.Path))
            {
                targetDirectory = config.Path;
                fileNameNoExt = $"Simulation_{stats.Id}_Export";
            }
            else
            {
                targetDirectory = Path.GetDirectoryName(config.Path);
                if (string.IsNullOrEmpty(targetDirectory)) targetDirectory = AppDomain.CurrentDomain.BaseDirectory;
                fileNameNoExt = Path.GetFileNameWithoutExtension(config.Path);
            }

            if (!Directory.Exists(targetDirectory)) throw new ExportException($"Directory not found: {targetDirectory}");

            var exportData = new
            {             
                Statistics = stats,     
                Customers = customers ?? new List<Customer>() // clients or empty list if it's null
            };

            // Serialize
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true, // better layout
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping  // no weird unicode codes but the real speciale cases
            }; 

            try
            {
                string jsonString = JsonSerializer.Serialize(exportData, options);

                // Writing
                string fullPath = Path.Combine(targetDirectory, $"{fileNameNoExt}.json");
                File.WriteAllText(fullPath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
