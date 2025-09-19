using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Query
{
    public interface IOrderQuery
    {
        Task<List<Order>> GetListOrderAsync(DateTime? from, DateTime? to , int? status);
        Task<Order> GetOrderByIdAsync(long orderId);

    }
}
