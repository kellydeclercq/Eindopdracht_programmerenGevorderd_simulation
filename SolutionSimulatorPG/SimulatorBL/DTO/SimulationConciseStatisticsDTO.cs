using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorBL.DTO
{
    public class SimulationConciseStatisticsDTO
    {
        public int Id { get; set; }
        public string Client { get; set; }
        public int AmountOfCustomers { get; set; }
        public string Country { get; set; }

        public SimulationConciseStatisticsDTO(int id, string client, int amountOfCustomers, string country)
        {
            Id = id;
            Client = client;
            AmountOfCustomers = amountOfCustomers;
            Country = country;
        }
    }
}
