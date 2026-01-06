using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorBL.Exceptions
{
    public class SimulatorReaderException : Exception
    {
        public SimulatorReaderException()
        {
        }

        public SimulatorReaderException(string? message) : base(message)
        {
        }

        public SimulatorReaderException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
