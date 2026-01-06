using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Configuration;
using SimulatorBL.Domain;
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Readers
{
    public class AddressReadertxt : IFileDataReader
    {
        const string extraDenSplit = "kommun";        //kommune: Denemark and kommun: Sweden 
        private List<string> toSkipHighwayType = AppConfig.Current.Settings.SkippedHighwayTypes;
        const string municipalityToSkip = "unknown";
        
        public List<Record> ReadFile(string filePath, int linesToSkip, char seperator, int nameColumn, 
            NameType nameType, Gender gender, string country, int? freqOrStreetColumn, int? highwaytypeColumn)
        {
            HashSet<Record> records = new HashSet<Record>();

            try
            {

                using (StreamReader reader = new StreamReader(filePath))
                {
                    for (int i = 0; i < linesToSkip; i++) { reader.ReadLine(); }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ss = line.Split(seperator);
                        //all data lower and trim
                        ss.Select(x => x.ToLower().Trim());

                        if (toSkipHighwayType.Contains(((string)ss[(int)highwaytypeColumn]))) continue;
                        if (ss[nameColumn].Contains(municipalityToSkip))
                            continue;

                        string municipality = (string)ss[nameColumn];           //munipality column
                        if (municipality.Contains($"{extraDenSplit}")) municipality = municipality.Replace($"{extraDenSplit}", "").Trim();
                        string street = (string)ss[(int)freqOrStreetColumn];       //street column

                        records.Add(new AddressRecord(0, municipality, street, country, DateTime.Now.Year));
                    }
                }
            }
            catch (Exception ex) { throw new SimulatorReaderException($"{typeof(AddressReadertxt)}: reading failed. \n {ex.Message}"); }

            return records.ToList();
        }

        
    }
}
