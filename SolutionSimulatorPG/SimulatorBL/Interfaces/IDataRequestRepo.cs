using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;

namespace SimulatorBL.Interfaces
{
    public interface IDataRequestRepo
    {
        List<string> GetAllCountries();
        List<MetaDataImportDTO> GetAllDatasets();
        List<string> GetAllMunicipalities(string country);
        List<SimulationInformation> GetAllSimulations();
        List<Customer> GetSpecificCustomers(int id);
        public List<Record> GetSpecificRecords(int year, string country, List<string> selectedMunicipalities);


    }
}
