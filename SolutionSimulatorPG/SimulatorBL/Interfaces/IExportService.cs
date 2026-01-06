using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Export;

namespace SimulatorBL.Interfaces
{
    public interface IExportService
    {
        public bool Export(SimulationInformation stats, ExportConfiguration config, List<Customer>? customers);
    }
}

