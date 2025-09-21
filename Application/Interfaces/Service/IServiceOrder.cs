using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IServiceOrder
    {
        Task<List<OrderDetailsResponse>> GetOrders(DateTime? from, DateTime? to, int? status);
        Task<OrderDetailsResponse> GetOrderById(long orderId);
        Task<OrderCreateReponse> CreateOrder(OrderRequest request);

    }
}
