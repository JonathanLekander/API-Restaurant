using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class NameException : Exception
    {
        public string Message;
        public NameException(string message) : base(message)
        {
            Message = message;
        }
    }
}
