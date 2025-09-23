using Application.DTOs.Request;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class PriceCalculator : IPriceCalculator
    {
        private readonly IDishQuery _dishQuery;

        public PriceCalculator(IDishQuery dishQuery)
        {
            _dishQuery = dishQuery;
        }

        public async Task<decimal> CalculateOrderTotalAsync(List<Items> items)
        {
            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var dish = await _dishQuery.GetDishByIdAsync(item.id);
                decimal itemPrice = dish.Price * item.quantity;
                totalAmount += itemPrice;
            }

            return totalAmount;
        }
    }
}
