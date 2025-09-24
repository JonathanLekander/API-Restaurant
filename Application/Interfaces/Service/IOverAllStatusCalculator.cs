using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IOverAllStatusCalculator 
    {
        int CalculateOverallStatus(IEnumerable<OrderItem> items);
    }
}
