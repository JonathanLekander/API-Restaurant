using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request
{
    public class DishRequest
    {
        public string name { get; set; }
        public string? description { get; set; }
        public double price { get; set; }
        public int category { get; set; }
        public string? image { get; set; }

    }
}
