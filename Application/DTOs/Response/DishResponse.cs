using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class DishResponse
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string? description { get; set; } = null;
        public double price { get; set; }
        public GenericResponse Category { get; set; }
        public string? image { get; set; }
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}
