using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.DTO;

namespace UnitTestSimulationSolution
{
    public class UnitTestMapper
    {
        // happy flow
        // Verify that all properties are copied correctly from the source to the DTO.
        [Fact]
        public void MapToConciseStatistics_ValidSimulationInfo_ReturnsCorrectDTO()
        {
            // We create a SimulationInformation object         
            var sourceData = new SimulationInformation(99, "Test Company", 18, 80, 12345,500, 100, 10, DateTime.Now, 35, new Dictionary<string, int>(), 
                new Dictionary<string, int>(), new Dictionary<string, int>(), new Dictionary<string, Dictionary<string, int>>() , "Netherlands", 2023);

            var result = DTOMapper.MapToConciseStatistics(sourceData);

            //Check if the result is not null
            Assert.NotNull(result);

            // Check if the 4 specific properties are mapped correctly
            Assert.Equal(99, result.Id);
            Assert.Equal("Test Company", result.Client);
            Assert.Equal(500, result.AmountOfCustomers);
            Assert.Equal("Netherlands", result.Country);
        }

        // Unhappy Path (null Check) 
        [Fact]
        public void MapToConciseStatistics_NullInput_ThrowsNullReferenceException()
        {
            SimulationInformation nullData = null;
            Assert.Throws<NullReferenceException>(() => DTOMapper.MapToConciseStatistics(nullData));
        }
    }
}
