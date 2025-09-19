using System;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Command
{
    public interface IOrderCommand
    {
        Task<long> CreateOrder(Order order);
        Task UpdateOrderItem(OrderItem orderItem);
        Task AddOrderItem(OrderItem orderItem);
    }
}
