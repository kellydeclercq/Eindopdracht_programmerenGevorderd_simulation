using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorBL.DTO
{
    public class MetaDataImportDTO
    {
        public int Id { get; set; }
        public DateTime ImportDate { get; set; }
        public string SourceFile { get; set; }
        public bool IsNamesData { get; set; }
        public bool IsAddresData { get; set; }
        public string Country { get; set; }
        public int versionYear { get; set; }

        public MetaDataImportDTO(int id, DateTime importDate, string sourceFile, bool isNamesData, bool isAddresData, string country, int versionYear)
        {
            Id = id;
            ImportDate = importDate;
            SourceFile = sourceFile;
            IsNamesData = isNamesData;
            IsAddresData = isAddresData;
            Country = country;
            this.versionYear = versionYear;
        }

    }
}
