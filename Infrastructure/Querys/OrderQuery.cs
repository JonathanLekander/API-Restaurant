using Application.Interfaces.Query;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Querys
{
    public class OrderQuery : IOrderQuery
    {
        private readonly RestaurantDbContext _context;

        public OrderQuery(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetListOrderAsync(DateTime? from, DateTime? to, int? status)
        {
            var query = _context.Order
                .Include(o => o.DeliveryType)
                .Include(o => o.OverallStatus)
                .Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Status)
                .AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(o => o.CreateDate >= from.Value);
            }

            if (to.HasValue) 
            {
                query = query.Where(o => o.CreateDate <= to.Value);
            }
              
            if (status.HasValue)
            {
                query = query.Where(o => o.OverallStatusId == status.Value);
            }

            return await query.OrderByDescending(o => o.CreateDate).ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(long id)
        {
            return await _context.Order
                .Include(o => o.DeliveryType)
                .Include(o => o.OverallStatus)
                .Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Status)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }
        public async Task<List<Order>> GetOrdersWithDishAsync(Guid dishId)
        {
            return await _context.Order
                .Include(o => o.OverallStatus)
                .Where(o => o.OrderItems.Any(oi => oi.DishId == dishId))
                .ToListAsync();

        }

    }
}
