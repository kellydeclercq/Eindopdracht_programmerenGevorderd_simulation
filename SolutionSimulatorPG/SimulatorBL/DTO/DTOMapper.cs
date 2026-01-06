using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorBL.DTO
{
    public static class DTOMapper
    {
        public static SimulationConciseStatisticsDTO MapToConciseStatistics(SimulationInformation data)
        {
            return new SimulationConciseStatisticsDTO(data.Id, data.ClientName, data.AmountOfCust, data.Country);
        }
    }
}
