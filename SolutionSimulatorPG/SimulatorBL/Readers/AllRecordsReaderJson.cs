using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;
using SimulatorBL.JsonMappers;

namespace SimulatorBL.Readers
{
    public class AllRecordsReaderJson : IFileDataReader
    {
       

        public List<Record> ReadFile(string filePath, int linesToSkip, char seperator, int nameColumn, NameType nameType, Gender gender, string country, int? frequencyColumn, int? highwaytypeColumn)
        {
            try
            {
                string jsonText = File.ReadAllText(filePath);      
                var rootData = JsonSerializer.Deserialize<AllRecordsRoot>(jsonText);
                var records = new List<Record>();

                if (rootData == null) return records;
                // NAMES
                if (rootData.Names != null)
                {

                    if (rootData.Names.MaleFirstNames != null)
                        foreach (var name in rootData.Names.MaleFirstNames)
                            records.Add(new NameRecord(0, name.ToLower().Trim(), NameType.Firstname, 1, Gender.Male, country, DateTime.Now.Year));

                    if (rootData.Names.FemaleFirstNames != null)
                        foreach (var name in rootData.Names.FemaleFirstNames)
                            records.Add(new NameRecord(0, name.ToLower().Trim(), NameType.Firstname, 1, Gender.Female, country, DateTime.Now.Year));


                    if (rootData.Names.MaleLastNames != null)
                        foreach (var name in rootData.Names.MaleLastNames)
                            records.Add(new NameRecord(0, name.ToLower().Trim(), NameType.Lastname, 1, Gender.Male, country, DateTime.Now.Year));


                    if (rootData.Names.FemaleLastNames != null)
                        foreach (var name in rootData.Names.FemaleLastNames)
                            records.Add(new NameRecord(0, name, NameType.Lastname, 1, Gender.Female, country, DateTime.Now.Year));
                }

                // ADDRESSES                                         
                if (rootData.Addresses != null)
                {
                    if (rootData.Addresses.Streets != null)
                    {
                        foreach (var streetName in rootData.Addresses.Streets)
                        {
                            records.Add(new AddressRecord(
                                0,
                                null, // Municipality is null
                                streetName.ToLower().Trim(),
                                country,
                                DateTime.Now.Year
                            ));
                        }
                    }


                    if (rootData.Addresses.CityNames != null)
                    {
                        foreach (var cityName in rootData.Addresses.CityNames)
                        {

                            records.Add(new AddressRecord(
                                0,
                                cityName.ToLower().Trim(),
                                null, // Street is null
                                country,
                                DateTime.Now.Year
                            ));
                        }
                    }
                }

                return records;
            }
            catch(Exception ex) { throw new SimulatorReaderException($"{typeof(AllRecordsReaderJson)}: reading file failed. \n {ex.Message}"); }
        }
    }
}
