using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class PriceException : Exception
    {
        public string Message;

        public PriceException(string message) : base(message)
        {
            Message = message;
        }
    }
}
