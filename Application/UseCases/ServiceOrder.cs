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
        private readonly IPriceCalculator _priceCalculator;
        private readonly IOverAllStatusCalculator _overAllStatusCalculator;
        public ServiceOrder(IOrderQuery orderQuery,IDishQuery dishQuery, IOrderCommand orderCommand, IDeliveryTypeQuery deliveryTypeQuery, IPriceCalculator priceCalculator, IOverAllStatusCalculator overAllStatusCalculator)
        {
            _orderQuery = orderQuery;
            _dishQuery = dishQuery;
            _orderCommand = orderCommand;
            _deliveryTypeQuery = deliveryTypeQuery;
            _priceCalculator = priceCalculator;
            _overAllStatusCalculator = overAllStatusCalculator;
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

               var dish = await _dishQuery.GetDishByIdAsync(item.id);

                if (dish == null || !dish.Available)
                {
                    throw new InvalidParameterException("El plato especificado no existe o no está disponible");
                }
            }

            decimal totalAmount = await _priceCalculator.CalculateOrderTotalAsync(request.items);

            var orderItems = new List<OrderItem>();

            foreach(var item in request.items)
            {
                var dish = await _dishQuery.GetDishByIdAsync(item.id);

                var orderItem = new OrderItem
                {
                    DishId = item.id,
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
                Price = totalAmount, //total calculado
                DeliveryTypeId = request.delivery.id,
                OverallStatusId = 1, 
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                OrderItems = orderItems

            };

            var orderId = await _orderCommand.CreateOrder(order);


            return new OrderCreateReponse
            {
                orderNumber = orderId,
                totalAmount = (double)totalAmount,
                createdAt = order.CreateDate
            };

        }
        public async Task<OrderUpdateReponse> UpdateOrder (long orderId, OrderUpdateRequest request)
        {

      
            var order = await _orderQuery.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("Orden no encontrada");

            if (order.OverallStatusId == 5) // 5 = Closed
                throw new InvalidParameterException("No se puede modificar una orden cerrada");

            
            foreach (var item in request.items)
            {
                if (item.quantity <= 0)
                    throw new InvalidParameterException("La cantidad debe ser mayor a 0");

                var dish = await _dishQuery.GetDishByIdAsync(item.id);
                if (dish == null || !dish.Available)
                    throw new InvalidParameterException("El plato especificado no está disponible");
            }

       
            foreach (var item in request.items)
            {
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.DishId == item.id);

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
                        DishId = item.id,
                        Quantity = item.quantity,
                        Notes = item.notes,
                        StatusId = 1, // Pending 
                        CreateDate = DateTime.UtcNow,
                        OrderId = order.OrderId
                    };
                    await _orderCommand.AddOrderItem(newOrderItem);
                }
            }

            
            order.OrderItems = await _orderQuery.GetOrderItemsByOrderIdAsync(order.OrderId);

           
            order.OverallStatusId = _overAllStatusCalculator.CalculateOverallStatus(order.OrderItems);
            order.UpdateDate = DateTime.UtcNow;

         
            var allItems = order.OrderItems
                .Select(oi => new Items { id = oi.DishId, quantity = oi.Quantity })
                .ToList();

            decimal totalAmount = await _priceCalculator.CalculateOrderTotalAsync(allItems);
            order.Price = totalAmount;

        
            await _orderCommand.UpdateOrder(order);

           
            return new OrderUpdateReponse
            {
                orderNumber = order.OrderId,
                totalAmount = (double)totalAmount,
                updateAt = DateTime.UtcNow
            };

        }
        public async Task<List<OrderDetailsResponse>> GetOrders(DateTime? from, DateTime? to, int? status)
        {
            if (status.HasValue && (status.Value < 1 || status.Value > 5))
            {
                throw new InvalidParameterException("El estado especificado no es válido");
            }

            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                throw new InvalidParameterException("Rango de fechas inválido");
            }

            var orders = await _orderQuery.GetListOrderAsync(from, to, status);

            var responseList = new List<OrderDetailsResponse>();

            foreach (var o in orders)
            {
                var response = new OrderDetailsResponse
                {
                    orderNumber = o.OrderId,
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
                    items = new List<OrderItemResponse>(),
                    createdAt = o.CreateDate,
                    updatedAt = o.UpdateDate
                };

                foreach (var oi in o.OrderItems)
                {
                    var itemResponse = new OrderItemResponse
                    {
                        id = oi.OrderItemId,
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
                    };

                    response.items.Add(itemResponse);
                }

                responseList.Add(response);
            }
            return responseList;
        }
        public async Task<OrderDetailsResponse> GetOrderById(long orderId)
        {
            var order = await _orderQuery.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                throw new NotFoundException("Orden no encontrada");
            }

            var itemsList = new List<OrderItemResponse>();
            foreach (var oi in order.OrderItems)
            {
                var itemResponse = new OrderItemResponse
                {
                    id = oi.OrderItemId,
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
                };

                itemsList.Add(itemResponse);
            }

            return new OrderDetailsResponse
            {
                orderNumber = order.OrderId,
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
                items = itemsList,
                createdAt = order.CreateDate,
                updatedAt = order.UpdateDate
            };
        }

        public async Task <OrderUpdateReponse> UpdateOrderItem(long orderId, long itemId, OrderItemUpdateRequest request)
        {
            var order = await _orderQuery.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("Orden no encontrada");
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.OrderItemId == itemId);
            if (orderItem == null)
            {
                throw new NotFoundException("Item de orden no encontrado");
            }

            if (request.status < 1 || request.status > 5)
            {
                throw new InvalidParameterException("El estado especificado no es válido");
            }


            orderItem.StatusId = request.status;
            order.UpdateDate = DateTime.UtcNow;

            await _orderCommand.UpdateOrderItem(orderItem);

        
            await UpdateOrderStatus(orderId);

            return new OrderUpdateReponse
            {
                orderNumber = orderId,
                totalAmount = (double)order.Price,
                updateAt = DateTime.UtcNow
            };
        }

        public async Task UpdateOrderStatus(long orderId)
        {
            var order = await _orderQuery.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("Orden no encontrada");
            }

            
            var orderItems = await _orderQuery.GetOrderItemsByOrderIdAsync(order.OrderId);
            var newStatus = _overAllStatusCalculator.CalculateOverallStatus(orderItems);

            // Actualizar solo el status de la orden
            order.OverallStatusId = newStatus;
            order.UpdateDate = DateTime.UtcNow;

            await _orderCommand.UpdateOrder(order);
        }


    }
}
