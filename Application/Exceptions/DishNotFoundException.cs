using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class DishNotFoundException : Exception
    {
        public string Message;

        public DishNotFoundException(string message) : base(message)
        {
            Message = message;
        }

    }
}
