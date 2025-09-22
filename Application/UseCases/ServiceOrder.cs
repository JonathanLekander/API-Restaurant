using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Command;
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
        private readonly IDishQuery _dishQuery;
        private readonly IOrderCommand _orderCommand;
        private readonly IDeliveryTypeQuery _deliveryTypeQuery;
        public ServiceOrder(IOrderQuery orderQuery,IDishQuery dishQuery, IOrderCommand orderCommand, IDeliveryTypeQuery deliveryTypeQuery)
        {
            _orderQuery = orderQuery;
            _dishQuery = dishQuery;
            _orderCommand = orderCommand;
            _deliveryTypeQuery = deliveryTypeQuery;
        }

        public async Task<OrderCreateReponse> CreateOrder (OrderRequest request)
        {
            var deliveryTypes = await _deliveryTypeQuery.GetListDeliveryTypeAsync();
            if (!deliveryTypes.Any(dt => dt.Id == request.delivery.id))
            {
                throw new InvalidParameterException("Debe especificar un tipo de entrega válido");
            }

            foreach (var item in request.items)
            {
                if (item.quantity <= 0)
                {
                    throw new InvalidParameterException("La cantidad debe ser mayor a 0");
                }

                Guid dishId;
                try
                {
                    dishId = Guid.Parse(item.id);
                }
                catch
                {
                    throw new InvalidParameterException("Formato de ID inválido");
                }

                var dish = await _dishQuery.GetDishByIdAsync(dishId);

                if (!dish.Available)
                {
                    throw new InvalidParameterException("El plato especificado no existe o no está disponible");
                }
            }

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach(var item in request.items)
            {
                var dish = await _dishQuery.GetDishByIdAsync(Guid.Parse(item.id));
                decimal itemPrice = dish.Price * item.quantity;
                totalAmount += itemPrice;

                var orderItem = new OrderItem
                {
                    DishId = dish.DishId,
                    Quantity = item.quantity,
                    Notes = item.notes,
                    StatusId = 1, // "Pending"
                    CreateDate = DateTime.UtcNow,
                };

                orderItems.Add(orderItem);

            }

            var order = new Order
            {
                DeliveryTo = request.delivery.to,
                Notes = request.notes,
                Price = totalAmount,
                DeliveryTypeId = request.delivery.id,
                OverallStatusId = 1, 
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                OrderItems = orderItems

            };

            var orderId = await _orderCommand.CreateOrder(order);

            return new OrderCreateReponse
            {
                orderNumber = (int)orderId,
                totalAmount = (double)totalAmount,
                createdAt = order.CreateDate
            };

        }
        public async Task<OrderUpdateReponse> UpdateOrder (OrderUpdateRequest request)
        {
            var activeOrder = await _orderQuery.GetActiveOrderAsync();
            if (activeOrder == null)
            {
                throw new NotFoundException("Orden no encontrada");
            }

            if (activeOrder.OverallStatus.Name == "Closed" || activeOrder.OverallStatus.Name == "Delivered")
            {
                throw new InvalidParameterException("No se puede modificar una orden cerrada o entregada");
            }

            foreach (var item in request.items)
            {
                if (item.quantity <= 0)
                {
                    throw new InvalidParameterException("La cantidad debe ser mayor a 0");
                }

                if (!Guid.TryParse(item.id, out var dishId))
                {
                    throw new InvalidParameterException("El plato especificado no existe");
                }

                var dish = await _dishQuery.GetDishByIdAsync(dishId);
                if (dish == null || !dish.Available)
                {
                    throw new InvalidParameterException("El plato especificado no existe o no está disponible");
                }
            }

            decimal totalAmount = 0;
            foreach (var item in request.items)
            {
                var dish = await _dishQuery.GetDishByIdAsync(Guid.Parse(item.id));
                decimal itemPrice = dish.Price * item.quantity;
                totalAmount += itemPrice;

                // Busco si el item ya existe en la orden
                var existingItem = activeOrder.OrderItems
                    .FirstOrDefault(oi => oi.DishId == Guid.Parse(item.id));

                if (existingItem != null)
                {
                   
                    existingItem.Quantity = item.quantity;
                    existingItem.Notes = item.notes;
                    await _orderCommand.UpdateOrderItem(existingItem);
                }
                else
                {
                    var newOrderItem = new OrderItem
                    {
                        DishId = Guid.Parse(item.id),
                        Quantity = item.quantity,
                        Notes = item.notes,
                        StatusId = 1, // Pending
                        CreateDate = DateTime.UtcNow,
                        OrderId = activeOrder.OrderId
                    };
                    await _orderCommand.AddOrderItem(newOrderItem);
                }
            }

            return new OrderUpdateReponse
            {
                orderNumber = (int)activeOrder.OrderId,
                totalAmount = (double)totalAmount,
                updatedAt = DateTime.UtcNow
            };

        }
        public async Task<List<OrderDetailsResponse>> GetOrders(DateTime? from, DateTime? to, int? status)
        {
            if (status.HasValue && (status.Value < 1 || status.Value > 5))
            {
                throw new InvalidParameterException("El status debe estar entre 1 y 5");
            }

            if (from.HasValue && to.HasValue && from.Value > to.Value)
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
