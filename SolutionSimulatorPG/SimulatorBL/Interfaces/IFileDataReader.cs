using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.Enum;

namespace SimulatorBL.Interfaces
{
    public interface IFileDataReader
    {
        public List<Record> ReadFile(string filePath, int linesToSkip, Char seperator,
            int nameColumn, NameType nameType, Gender gender, string country, int? frequencyColumn, int? highwaytypeColumn);
    }
}
