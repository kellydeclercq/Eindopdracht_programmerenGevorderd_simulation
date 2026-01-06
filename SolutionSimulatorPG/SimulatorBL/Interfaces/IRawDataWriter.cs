using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.Enum;

namespace SimulatorBL.Interfaces
{
    public interface IRawDataWriter
    {
        public bool WriteRecordsToDB(List<NameRecord> firstNames, List<NameRecord> lastNames, List<AddressRecord> addresses);
        public bool WriteImportMetaDataToDB(bool isNames, bool isAddresses, string path, string country);
    }
}
