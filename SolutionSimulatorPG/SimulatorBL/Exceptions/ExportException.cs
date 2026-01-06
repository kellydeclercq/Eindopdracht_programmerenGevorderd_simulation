using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorBL.Exceptions
{
    public class ExportException : Exception
    {
        public ExportException()
        {
        }

        public ExportException(string? message) : base(message)
        {
        }

        public ExportException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
