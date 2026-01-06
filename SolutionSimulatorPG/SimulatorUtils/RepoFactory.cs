using System.Net.NetworkInformation;
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;
using SimulatorDL;

namespace SimulatorUtils
{
    public static class RepoFactory
    {
        public static IRawDataWriter GetRepo()
        {
            return new RawDataWriterRepo();
        }

        public static SimulatorBL.Interfaces.IDataRequestRepo GetRequestRepo()
        {
            return new DataRequestRepo();
        }

        public static ICustomerGenerator GetSimulator(SimulatorBL.Manager.DataRequestService requestService, int seed)
        {
            return  new SimulatorService(GetWriterRepo(), requestService, seed);
        }
     

        public static IWriterRepo GetWriterRepo() 
        {
            return new DataWriterRepo();
        }
    }
}
