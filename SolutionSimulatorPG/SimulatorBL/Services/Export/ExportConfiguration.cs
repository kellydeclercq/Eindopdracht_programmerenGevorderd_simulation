using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;

namespace SimulatorBL.Services.Export
{
    public class ExportConfiguration
    {
        public FileType FileType { get;  set; }
        public string? TextSeparator { get;  set; }
        public bool IncludeFullInformation { get;  set; }
        public string Path { get; set; }

        public ExportConfiguration(FileType fileType, string? textSeparator, bool includeFullInformation, string Path)
        {
            FileType = fileType;
            TextSeparator = textSeparator;
            IncludeFullInformation = includeFullInformation;
        }

        public ExportConfiguration()
        {
        }
    }
}
