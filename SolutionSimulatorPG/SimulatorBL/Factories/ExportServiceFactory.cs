using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;
using SimulatorBL.Services.Export;

namespace SimulatorBL.Factories
{
    public static class ExportServiceFactory
    {
        public static IExportService GetExportService(FileType fileType)
        {
            if (fileType == FileType.TextorCSV) return new TextExportService();
            else if (fileType == FileType.Json) return new JsonExportService();
            else throw new ExportException("No exportService found.");
        }
    }
}
