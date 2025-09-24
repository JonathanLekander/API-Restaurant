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

            // Si algún item está cancelado (5), la orden es cancelada
            if (items.Any(x => x.StatusId == 5))
                return 5;

            // Si todos los items están entregados (4), la orden es entregada
            if (items.All(x => x.StatusId == 4))
                return 4;

            // Si hay algún item listo (3)
            if (items.Any(x => x.StatusId == 3))
                return 3;

            // Si hay algún item en preparación (2)
            if (items.Any(x => x.StatusId == 2))
                return 2;

            // Por defecto, pendiente (1)
            return 1;
        }
    }
    
}
