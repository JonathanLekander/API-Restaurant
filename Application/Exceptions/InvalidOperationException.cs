using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class InvalidOperationException : Exception
    {
        public string Message;
        public InvalidOperationException(string message) : base(message)
        {
            Message = message;
        }
    }
}
