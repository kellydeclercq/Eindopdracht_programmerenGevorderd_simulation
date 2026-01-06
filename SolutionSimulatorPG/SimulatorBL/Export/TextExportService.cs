using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Export
{
    public class TextExportService : IExportService
    {
        public bool Export(SimulationInformation stats, ExportConfiguration config, List<Customer>? customers)
        {
            if (stats == null) throw new ExportException($"{nameof(stats)} is NULL");
            if (string.IsNullOrWhiteSpace(config.Path)) throw new ExportException("Export path can't be null or whitespace.");

            string separator = config.TextSeparator ?? ";";
            string extension = ".csv";

            string targetDirectory;
            string fileNameNoExt;

            // is it a directory: make new file
            if (Directory.Exists(config.Path))
            {
                targetDirectory = config.Path;
                fileNameNoExt = $"Simulation_{stats.Id}";
            }
            else // we do have a file 
            {
                targetDirectory = Path.GetDirectoryName(config.Path);
                fileNameNoExt = Path.GetFileNameWithoutExtension(config.Path);
            }

            // does directory exist?
            if (!Directory.Exists(targetDirectory))
            {
                throw new ExportException($"Directory not found: {targetDirectory}");
            }

            try
            {              
                if (config.IncludeFullInformation)
                {
                    // File 1: Summary
                    string summaryPath = Path.Combine(targetDirectory, $"{fileNameNoExt}_Summary{extension}");
                    string summaryContent = GetStatsCsvContent(stats, separator);
                    File.WriteAllText(summaryPath, summaryContent, new UTF8Encoding(true));

                    // File 2: stats + client                    
                    if (customers != null && customers.Count > 0)
                    {
                        string fullPath = Path.Combine(targetDirectory, $"{fileNameNoExt}_FullData{extension}");
                        
                        StringBuilder fullContent = new StringBuilder();                     
                        fullContent.AppendLine("--- CUSTOMER DATA ---");
                        fullContent.Append(GetCustomersCsvContent(customers, separator));
                        File.WriteAllText(fullPath, fullContent.ToString(), new UTF8Encoding(true));
                    }
                }
                else
                {
                    // Only stats
                    string finalPath = config.Path;
                    if (Directory.Exists(finalPath))
                    {   
                        //True: it's a map, so make own file
                        string defaultFileName = $"Simulation_{stats.Id}_Stats.csv";
                        finalPath = Path.Combine(finalPath, defaultFileName);
                    }
                    
                    string content = GetStatsCsvContent(stats, separator);
                    File.WriteAllText(finalPath, content, new UTF8Encoding(true));
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
   
        private string GetStatsCsvContent(SimulationInformation stats, string sep)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Id{sep}ClientName{sep}AmountOfCustomers{sep}Country{sep}MinAge{sep}MaxAge{sep}CreationDate");
            sb.AppendLine($"{stats.Id}{sep}{stats.ClientName}{sep}{stats.AmountOfCust}{sep}{stats.Country}{sep}{stats.MinAge}{sep}{stats.MaxAge}{sep}{stats.CreationDate}");
            return sb.ToString();
        }

     
        private string GetCustomersCsvContent(List<Customer> customers, string sep)
        {
            StringBuilder sb = new StringBuilder();

            // Header 
            sb.AppendLine($"Name{sep}Lastname{sep}Gender{sep}Street{sep}HouseNumber{sep}Municipality{sep}Country{sep}BirthDate");
            // Data Rows
            foreach (var c in customers)
            {
                sb.AppendLine($"{c.Name}{sep}{c.Lastname}{sep}{c.Gender}{sep}{c.Street}{sep}{c.HouseNumber}{sep}{c.Municipality}{sep}{c.Country}{sep}{c.BirthDate.ToShortDateString()}");
            }

            return sb.ToString();
        }

    }
    
}
