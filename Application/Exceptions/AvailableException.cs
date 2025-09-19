using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class AvailableException : Exception
    {
        public string Message;
        public AvailableException(string message) : base()
        {
            this.Message = message;
        }
    }
}
