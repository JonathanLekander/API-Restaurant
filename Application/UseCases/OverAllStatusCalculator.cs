using Application.Interfaces.Service;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class OverAllStatusCalculator : IOverAllStatusCalculator
    {
        public int CalculateOverallStatus(IEnumerable<OrderItem> items)
        {
            if (items == null || !items.Any())
                return 1; // Pending por defecto

            // 1. Closed tiene prioridad máxima
            if (items.All(i => i.StatusId == 5))
                return 5; // Closed

            // 2. Delivery
            if (items.All(i => i.StatusId == 4 || i.StatusId == 5))
                return 4; // Delivery

            // 3. Ready
            if (items.All(i => i.StatusId == 3 || i.StatusId == 4 || i.StatusId == 5))
                return 3; // Ready

            // 4. In progress
            if (items.Any(i => i.StatusId == 2))
                return 2; // In progress

            // 5. Pending por defecto
            return 1;
        }
    }
}
