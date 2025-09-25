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
                return 1; // Pending

            // Toma el STATUS MÁS BAJO de los items
            var minStatus = items.Min(x => x.StatusId);

            return minStatus;
        }
    }
    
}
