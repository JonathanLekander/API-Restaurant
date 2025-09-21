using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class ServiceOrder : IServiceOrder
    {
        private readonly IOrderQuery _orderQuery;
        public ServiceOrder(IOrderQuery orderQuery)
        {
            _orderQuery = orderQuery;
        }

        public async Task<List<OrderDetailsResponse>> GetOrders(DateTime? from, DateTime? to, int? status)
        {
            if (from.HasValue && to.HasValue && from > to)
            {
                throw new InvalidParameterException("Rango de fechas inválido");
            }

            var orders = await _orderQuery.GetListOrderAsync(from, to, status);

            return orders.Select(o => new OrderDetailsResponse
            {
                orderNumber = (int)o.OrderId,
                totalAmount = (double)o.Price,
                deliveryTo = o.DeliveryTo,
                notes = o.Notes,
                status = new GenericResponse
                {
                    id = o.OverallStatus.Id,
                    name = o.OverallStatus.Name
                },
                deliveryType = new GenericResponse
                {
                    id = o.DeliveryType.Id,
                    name = o.DeliveryType.Name
                },
                items = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    id = (int)oi.OrderItemId,
                    quantity = oi.Quantity,
                    notes = oi.Notes,
                    status = new GenericResponse
                    {
                        id = oi.Status.Id,
                        name = oi.Status.Name
                    },
                    dish = new DishShortResponse
                    {
                        id = oi.Dish.DishId,
                        name = oi.Dish.Name,
                        image = oi.Dish.ImageUrl
                    }
                }).ToList(),

                createdAt = o.CreateDate,
                updatedAt = o.UpdateDate
            }).ToList();
        }
        public async Task<OrderDetailsResponse> GetOrderById(long orderId)
        {
            var order = await _orderQuery.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                throw new NotFoundException("Orden no encontrada");
            }

            return new OrderDetailsResponse
            {
                orderNumber = (int)order.OrderId,
                totalAmount = (double)order.Price,
                deliveryTo = order.DeliveryTo,
                notes = order.Notes,
                status = new GenericResponse
                {
                    id = order.OverallStatus.Id,
                    name = order.OverallStatus.Name
                },
                deliveryType = new GenericResponse
                {
                    id = order.DeliveryType.Id,
                    name = order.DeliveryType.Name
                },
                items = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    id = (int)oi.OrderItemId,
                    quantity = oi.Quantity,
                    notes = oi.Notes,
                    status = new GenericResponse
                    {
                        id = oi.Status.Id,
                        name = oi.Status.Name
                    },
                    dish = new DishShortResponse
                    {
                        id = oi.Dish.DishId,
                        name = oi.Dish.Name,
                        image = oi.Dish.ImageUrl
                    }
                }).ToList(),
                createdAt = order.CreateDate,
                updatedAt = order.UpdateDate
            };
        }

    }
}
