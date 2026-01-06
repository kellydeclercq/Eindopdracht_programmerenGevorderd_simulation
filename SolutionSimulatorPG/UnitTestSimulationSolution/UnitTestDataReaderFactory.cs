using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;
using SimulatorBL.Factories;
using SimulatorBL.Readers;

namespace UnitTestSimulationSolution
{
    public class UnitTestDataReaderFactory
    {
        //happy flow
        [Theory]
        [InlineData(NameType.Firstname)]
        [InlineData(NameType.Lastname)]
        public void GetDatareader_TextAndNameType_ReturnsNameReadertxt(NameType nameType)
        {
            var result = DataReaderFactory.GetDatareader(nameType, FileType.TextorCSV, "AnyCountry");
            Assert.IsType<NameReadertxt>(result);
        }

        [Fact]
        public void GetDatareader_JsonAndCzechRepublic_ReturnsAllRecordsReaderJson()
        {
            var result = DataReaderFactory.GetDatareader(NameType.Firstname, FileType.Json, "CzechRepublic");
            Assert.IsType<AllRecordsReaderJson>(result);
        }

        [Fact]
        public void GetDatareader_JsonAndPoland_ReturnsNameReaderJson()
        {
            var result = DataReaderFactory.GetDatareader(NameType.Firstname, FileType.Json, "Poland");
            Assert.IsType<NameReaderJson>(result);
        }


        //unhappy flow
        [Fact]
        public void GetDatareader_UnsupportedCountryForJson_ThrowsNotSupportedException()
        {
            string unsupportedCountry = "Belgium";
            var ex = Assert.Throws<NotSupportedException>(() =>
                DataReaderFactory.GetDatareader(NameType.Firstname, FileType.Json, unsupportedCountry));            
        }
    }
}
