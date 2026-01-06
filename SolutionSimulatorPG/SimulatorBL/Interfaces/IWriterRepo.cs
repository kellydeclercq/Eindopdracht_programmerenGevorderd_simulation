using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;

namespace SimulatorBL.Interfaces
{
    public interface IWriterRepo
    {
        public bool WriteDatasetToDB(SimulationInformation sim, List<Customer> customers);
    }
}
