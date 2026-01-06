using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Services.Export
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

            // determine directory + filename
            if (Directory.Exists(config.Path))
            {
                targetDirectory = config.Path;
                fileNameNoExt = $"Simulation_{stats.Id}";
            }
            else
            {
                targetDirectory = Path.GetDirectoryName(config.Path);
                if (string.IsNullOrEmpty(targetDirectory)) targetDirectory = AppDomain.CurrentDomain.BaseDirectory;
                fileNameNoExt = Path.GetFileNameWithoutExtension(config.Path);
            }

            if (!Directory.Exists(targetDirectory))
            {
                throw new ExportException($"Directory not found: {targetDirectory}");
            }

            try
            {
                // Haal de content op (nu inclusief dictionaries)
                string summaryContent = GetStatsCsvContent(stats, separator);

                if (config.IncludeFullInformation)
                {
                    // Summary (Stats + Dictionaries)
                    string summaryPath = Path.Combine(targetDirectory, $"{fileNameNoExt}_Summary{extension}");
                    File.WriteAllText(summaryPath, summaryContent, new UTF8Encoding(true));

                    // clientlist
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
                    // all stats in one file
                    string finalPath = config.Path;
                    if (Directory.Exists(finalPath))
                    {
                        string defaultFileName = $"Simulation_{stats.Id}_Stats.csv";
                        finalPath = Path.Combine(finalPath, defaultFileName);
                    }
                    File.WriteAllText(finalPath, summaryContent, new UTF8Encoding(true));
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

            // GENERAL info
            sb.AppendLine("--- GENERAL INFORMATION ---");
            sb.AppendLine(
                $"Id{sep}" +
                $"ClientName{sep}" +
                $"Year{sep}" +
                $"Country{sep}" +
                $"AmountOfCustomers{sep}" +
                $"MinAge{sep}" +
                $"MaxAge{sep}" +
                $"AvgAgeStart{sep}" +
                $"AvgAgeNow{sep}" +
                $"RandomSeed{sep}" +
                $"MaxHouseNr{sep}" +
                $"LetterPerc{sep}" +
                $"CreationDate"
            );

            sb.AppendLine(
                $"{stats.Id}{sep}" +
                $"{stats.ClientName}{sep}" +
                $"{stats.Year}{sep}" +
                $"{stats.Country}{sep}" +
                $"{stats.AmountOfCust}{sep}" +
                $"{stats.MinAge}{sep}" +
                $"{stats.MaxAge}{sep}" +
                $"{stats.AverageAgeOriginal}{sep}" +
                $"{stats.AverageAgeNow}{sep}" +
                $"{stats.RandomSeed}{sep}" +
                $"{stats.MaxHouseNr}{sep}" +
                $"{stats.HouseNumberLetterPercentage}{sep}" +
                $"{stats.CreationDate}"
            );

            // ADDING DICTIONARIES  
            // Munic + percentages
            sb.Append(GetDictionarySection(stats.MunicipalityPerc, "Requested Distribution", "Municipality", "Percentage (%)", sep));

            // clients per munic
            sb.Append(GetDictionarySection(stats.ClientsPerMunicipality, "Actual Clients per Municipality", "Municipality", "Count", sep));

            //straats per munic
            sb.Append(GetDictionarySection(stats.StreetsPerMunicipality, "Streets Found per Municipality", "Municipality", "Streets Count", sep));

            return sb.ToString();
        }

    
        private string GetDictionarySection(Dictionary<string, int> data, string sectionTitle, string col1Header, string col2Header, string sep)
        {
            if (data == null || data.Count == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"--- {sectionTitle.ToUpper()} ---"); 
            sb.AppendLine($"{col1Header}{sep}{col2Header}");   // Headers

            foreach (var kvp in data)
            {
                sb.AppendLine($"{kvp.Key}{sep}{kvp.Value}");
            }

            return sb.ToString();
        }

        private string GetCustomersCsvContent(List<Customer> customers, string sep)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name{sep}Lastname{sep}Gender{sep}Street{sep}HouseNumber{sep}Municipality{sep}Country{sep}BirthDate");

            foreach (var c in customers)
            {
                sb.AppendLine($"{c.Name}{sep}{c.Lastname}{sep}{c.Gender}{sep}{c.Street}{sep}{c.HouseNumber}{sep}{c.Municipality}{sep}{c.Country}{sep}{c.BirthDate.ToShortDateString()}");
            }

            return sb.ToString();
        }
    }

}
    

