using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Interfaces;

namespace SimulatorBL.Manager
{
    public class DataRequestService
    {
        IDataRequestRepo _requestRepo;

        public DataRequestService(IDataRequestRepo dataRequest)
        {
            _requestRepo = dataRequest;
        }

        public List<string> GetAllCountries()
        {
            return _requestRepo.GetAllCountries();
        }

        public List<MetaDataImportDTO> GetAllDatasets()
        {
            return _requestRepo.GetAllDatasets();
        }

        public List<string> GetAllMunicipalities(string? country)
        {
            return _requestRepo.GetAllMunicipalities(country);
        }

        public List<SimulationInformation> GetAllSimulations()
        {
            return _requestRepo.GetAllSimulations();
        }

        public List<Customer> GetSpecificCustomers(int id)
        {
           return _requestRepo.GetSpecificCustomers(id);
        }

        public List<Record> GetSpecificRecords(int year, string country, List<string> selecMunic)
        {
           return _requestRepo.GetSpecificRecords(year, country, selecMunic);
        }


    }
}
