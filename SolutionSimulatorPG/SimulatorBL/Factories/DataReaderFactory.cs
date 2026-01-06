using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Configuration;
using SimulatorBL.Enum;
using SimulatorBL.Interfaces;
using SimulatorBL.Readers;

namespace SimulatorBL.Factories
{
    public static class DataReaderFactory
    {
        private static readonly List<string> countriesWithAllRecordsInJson = AppConfig.Current.Settings.CountriesWithAllRecords;
        private static readonly List<string> countriesWithOnlyNameRecordsInJson = AppConfig.Current.Settings.CountriesWithOnlyNameRecords;

        public static IFileDataReader GetDatareader(NameType type, FileType fileType, string country)
        {
            country = country.Trim().ToLower();

            if ((type == NameType.Firstname || type == NameType.Lastname) && fileType == FileType.TextorCSV) return new NameReadertxt();
            else  if (type == NameType.Address && fileType == FileType.TextorCSV) return new AddressReadertxt();
            else if (countriesWithAllRecordsInJson.Contains(country.ToLower()) && fileType == FileType.Json) return new AllRecordsReaderJson();
            else if (countriesWithOnlyNameRecordsInJson.Contains(country) && fileType == FileType.Json) return new NameReaderJson();
            else throw new NotSupportedException($"Unsupported combination: {type} and {fileType}");
            

        }
    }
} 
