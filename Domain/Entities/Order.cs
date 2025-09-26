using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public long OrderId { get; set; }
        public string DeliveryTo { get; set; }
        public string Notes { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        [Column("OverallStatus")]
        public int OverallStatusId { get; set; }
        public Status OverallStatus { get; set; }

        [Column("DeliveryType")]
        public int DeliveryTypeId { get; set; }
        public DeliveryType DeliveryType { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
