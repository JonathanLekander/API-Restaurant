using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class OrderCreateReponse
    {
        public int orderNumber { get; set; }
        public double totalAmount { get; set; }
        public DateTime createdAt { get; set; }
    }
}
