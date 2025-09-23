using Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IPriceCalculator 
    {
        Task<decimal> CalculateOrderTotalAsync(List<Items> items);
    }
}
