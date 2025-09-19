using Application.Interfaces.Command;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands
{
    public class OrderCommand : IOrderCommand
    {
        private readonly RestaurantDbContext _context;

        public OrderCommand(RestaurantDbContext context)
        {
            _context = context;
        }
        
        public async Task <long> CreateOrder(Order order)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return order.OrderId;

        }
        public async Task UpdateOrderItem(OrderItem orderItem) 
        {
            _context.OrderItem.Update(orderItem); 
            await _context.SaveChangesAsync();
        }

    }
}
