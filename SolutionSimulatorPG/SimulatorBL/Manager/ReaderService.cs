using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.Enum;
using SimulatorBL.Factories;
using SimulatorBL.Interfaces;
using SimulatorBL.JsonMappers;

namespace SimulatorBL.Manager
{
    public class ReaderService
    {
        IFileDataReader _reader;
        IRawDataWriter _repo;

        public ReaderService(IRawDataWriter dataRepository)
        {            
            _repo = dataRepository;
        }

        public bool Read(string filePath, int linesToSkip, char seperator, int nameColumn, Gender gender, FileType fileType, 
            NameType nameType, string country, int? frequencyColumn, int? highwaytypeColumn)
        {
            List<Record> records;
            bool readingSucces;
            bool uploadSucces;
            try
            {
                _reader = DataReaderFactory.GetDatareader(nameType, fileType, country); 
                records = _reader.ReadFile(filePath, linesToSkip, seperator, nameColumn, nameType, gender, country, frequencyColumn, highwaytypeColumn); 
                readingSucces = records.Count > 0 || records != null;
                uploadSucces = false;

                if (records != null)
                {
                    List<NameRecord> firstNames = records.OfType<NameRecord>().Where(x => x.nameType == NameType.Firstname).ToList();
                    List<NameRecord> lastNames = records.OfType<NameRecord>().Where(x => x.nameType == NameType.Lastname).ToList();
                    List<AddressRecord> addresses = records.OfType<AddressRecord>().ToList();

                    bool isNames = firstNames.Count > 0 || lastNames.Count > 0;
                    bool isAddresses = addresses.Count > 0;

                    // trimming path after reading to upload short version in DB
                    filePath = Path.GetFileName(filePath);
                    uploadSucces = _repo.WriteRecordsToDB(firstNames, lastNames, addresses)
                                    && _repo.WriteImportMetaDataToDB(isNames, isAddresses, filePath, country.ToString());

                }
            }catch { return false; }


            return readingSucces && uploadSucces;

        }
    }
}
