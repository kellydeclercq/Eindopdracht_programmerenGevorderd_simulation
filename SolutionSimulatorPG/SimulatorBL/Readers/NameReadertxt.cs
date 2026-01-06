using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Readers
{
    public class NameReadertxt : IFileDataReader
    {
            
        public NameReadertxt()
        {
        }

        public List<Record> ReadFile(string filePath, int linesToSkip, Char seperator, 
            int nameColumn, NameType nameType, Gender gender, string country, int? frequencyColumn, int? highwaytypeColumn)
        {
            try
            {
                List<Record> records = new List<Record>();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    for (int i = 0; i < linesToSkip; i++) { reader.ReadLine(); }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ss = line.Split(seperator);
                        ss.Select(x => x.Trim().ToLower());

                        //Skipping ending of pages
                        if (string.IsNullOrWhiteSpace((string)ss[nameColumn]))
                            break;

                        string name = (string)ss[nameColumn].Trim().ToLower();
                        int frequency = 0;
                        if (frequencyColumn == null) frequency = 1;
                        else
                        {
                            // american way of writing numbers disable
                            string rawVal = ss[(int)frequencyColumn].Replace(".", "").Trim().ToLower();

                            // No try catch, this asks to much of system (slow)
                            if (!int.TryParse(rawVal, out frequency)) continue;

                        }
                        records.Add(new NameRecord(0, name, nameType, frequency, gender, country, DateTime.Now.Year));
                    }
                }


                return records;
            }
            catch (Exception ex) { throw new SimulatorReaderException($"{typeof(NameReadertxt)}: Reading file failed \n {ex.Message}"); }
        }

    }
}
