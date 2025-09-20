using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class DishInUseException : Exception
    {
        public string Message;
        public DishInUseException(string message) : base(message)
        {
            Message = message;
        }
    }
}
