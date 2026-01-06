using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Export;
using SimulatorBL.Factories;

namespace UnitTestSimulationSolution
{
    public class UnitTestExportServiceFactory
    {
        [Fact] //happy flow
        public void GetExportService_TextType_ReturnsTextExportService()
        {

            var result = ExportServiceFactory.GetExportService(FileType.TextorCSV);
            Assert.IsType<TextExportService>(result);
        }

        [Fact]
        public void GetExportService_JsonType_ReturnsJsonExportService()
        {
            var result = ExportServiceFactory.GetExportService(FileType.Json);
            Assert.IsType<JsonExportService>(result);
        }

        // unhappy flow
        [Fact]
        public void GetExportService_UnknownType_ThrowsExportException()
        {
            var invalidType = (FileType)999;
            Assert.Throws<ExportException>(() => ExportServiceFactory.GetExportService(invalidType));
        }

    }
}
