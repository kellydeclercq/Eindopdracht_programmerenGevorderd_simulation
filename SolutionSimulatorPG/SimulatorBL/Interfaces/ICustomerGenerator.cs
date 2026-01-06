using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.DTO;

namespace SimulatorBL.Interfaces
{
    public interface ICustomerGenerator
    {
       
        public bool StartSimulation(string clientName, int year, string country, int minAge, int maxAge, int numberOfCust,
            Dictionary<string, int> municipalityPerc, int maxHousenr, int percentageLetters, ObservableCollection<SimulationInformation> simulationInformationDTOs);
    }
}
