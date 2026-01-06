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
    public class NameReaderJson : IFileDataReader
    {

        public List<Record> ReadFile(string filePath, int linesToSkip, char seperator, int nameColumn, NameType nameType, Gender gender, string country, int? frequencyColumn, int? highwaytypeColumn)
        {
            try
            {
                string jsonText = File.ReadAllText(filePath);
                var rootData = JsonSerializer.Deserialize<NameRoot>(jsonText);
                var records = new List<Record>();


                if (rootData?.nameList == null) return records;


                if (rootData.nameList.MaleFirstNames != null)
                {
                    foreach (var name in rootData.nameList.MaleFirstNames)
                    {
                        records.Add(new NameRecord(0, name.ToLower().Trim(), NameType.Firstname, 1, Gender.Male, country, DateTime.Now.Year));

                    }
                }

                if (rootData.nameList.FemaleFirstNames != null)
                {
                    foreach (var name in rootData.nameList.FemaleFirstNames)
                    {
                        records.Add(new NameRecord(0, name.ToLower().Trim(), NameType.Firstname, 1, Gender.Female, country, DateTime.Now.Year));
                    }
                }


                if (rootData.nameList.LastNames != null)
                {
                    foreach (var name in rootData.nameList.LastNames)
                    {
                        records.Add(new NameRecord(0, name.ToLower().Trim(), NameType.Lastname, 1, Gender.Unknown, country, DateTime.Now.Year));
                    }
                }

                return records;
            }
            catch (Exception ex) { throw new SimulatorReaderException($"{typeof(NameReaderJson)}: reading file failed. \n {ex.Message}"); }
        }
    }
}
