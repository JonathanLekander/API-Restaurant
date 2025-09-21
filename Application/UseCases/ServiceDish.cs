using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Command;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using Domain.Entities;

namespace Application.UseCases
{
    public class ServiceDish : IServiceDish
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;
        private readonly IOrderQuery _orderQuery;

        public ServiceDish(IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery, IOrderQuery orderQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
            _orderQuery = orderQuery;
        }
        public async Task<DishResponse> CreateDish(DishRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.name))
            {
                throw new InvalidParameterException("El nombre del plato es obligatorio");
            }

            var existingDish = await _query.GetDishByNameAsync(request.name);
            if (existingDish != null)
            {
                throw new NameException("Ya existe un plato con ese nombre");
            }

            if (request.price <= 0)
            {
                throw new PriceException("El precio debe ser mayor a cero");
            }

            var categoryExists = await _categoryQuery.GetCategoryByIdAsync(request.category);
            if (categoryExists == null)
            {
                throw new InvalidParameterException("Categoría inválida");
            }


            Dish dish = new Dish
            {
                DishId = Guid.NewGuid(),
                Name = request.name,
                Description = request.description,
                Price = (decimal)request.price,
                CategoryId = request.category,
                ImageUrl = request.image,
                Available = true,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _command.CreateDish(dish);
            var createdDish = await _query.GetDishByIdAsync(dish.DishId);

            return new DishResponse
            {
                id = dish.DishId,
                name = dish.Name,
                description = dish.Description,
                price = (double)dish.Price,
                Category = new GenericResponse
                {
                    id = dish.Category.Id,
                    name = dish.Category.Name
                },
                image = dish.ImageUrl,
                isActive = dish.Available,
                createdAt = dish.CreateDate,
                updatedAt = dish.UpdateDate
            };
        }
        public async Task<List<DishResponse>> GetDishes(string? name, int? category, string sortByPrice, bool? onlyActive)
        {
            if (category.HasValue)
            {
                var categoryExists = await _categoryQuery.GetCategoryByIdAsync(category.Value);
                if (categoryExists == null)
                {
                    throw new InvalidParameterException("Categoría inválida");
                }
            }

            if (!string.IsNullOrEmpty(sortByPrice))
            {
                if (sortByPrice.ToLower() != "asc" && sortByPrice.ToLower() != "desc")
                {
                    throw new InvalidParameterException("Parámetros de ordenamiento inválidos");
                }
            }


            var dishes = await _query.GetListDishAsync(name, category, sortByPrice, onlyActive);

            return dishes.Select(d => new DishResponse
            {
                id = d.DishId,
                name = d.Name,
                description = d.Description,
                price = (double)d.Price,
                Category = new GenericResponse
                {
                    id = d.CategoryId,
                    name = d.Category?.Name
                },
                image = d.ImageUrl,
                isActive = d.Available,
                createdAt = d.CreateDate,
                updatedAt = d.UpdateDate
            }).ToList();
        }


        public async Task<DishResponse> UpdateDish(Guid id, DishUpdateRequest request)
        {
            var existingDish = await _query.GetDishByIdAsync(id);
            if (existingDish == null)
            {
                throw new NotFoundException("Plato no encontrado");
            }

            if (existingDish.Name != request.name)
            {
                var dishNameRepeat = await _query.GetDishByNameAsync(request.name);
                if (dishNameRepeat != null && dishNameRepeat.DishId != id)
                {
                    throw new NameException("Ya existe un plato con ese nombre");

                }
            }
            if (request.price <= 0)
            {
                throw new PriceException("El precio debe ser mayor a cero");
            }
            var categoryExists = await _categoryQuery.GetCategoryByIdAsync(request.category);
            if (categoryExists == null)
            {
                throw new InvalidParameterException("La categoría especificada no existe");
            }

            existingDish.Name = request.name;
            existingDish.Description = request.description;
            existingDish.Price = (decimal)request.price;
            existingDish.CategoryId = request.category;
            existingDish.ImageUrl = request.image;
            existingDish.Available = request.isActive;
            existingDish.UpdateDate = DateTime.Now;

            await _command.UpdateDish(existingDish);
            var updatedDish = await _query.GetDishByIdAsync(id);

            return new DishResponse
            {
                id = updatedDish.DishId,
                name = updatedDish.Name,
                description = updatedDish.Description,
                price = (double)updatedDish.Price,
                Category = new GenericResponse
                {
                    id = updatedDish.CategoryId,
                    name = updatedDish.Category.Name
                },
                image = updatedDish.ImageUrl,
                isActive = updatedDish.Available,
                createdAt = updatedDish.CreateDate,
                updatedAt = updatedDish.UpdateDate
            };

        }

        public async Task<DishResponse> GetDishById(Guid id)
        {
            var existingDish = await _query.GetDishByIdAsync(id);

            if (existingDish == null)
            {
                throw new NotFoundException("Plato no encontrado");
            }

            if (!existingDish.Available)
            {
                throw new AvailableException("El plato no está disponible");
            }

            return new DishResponse
            {
                id = existingDish.DishId,
                name = existingDish.Name,
                description = existingDish.Description,
                price = (double)existingDish.Price,
                Category = new GenericResponse
                {
                    id = existingDish.CategoryId,
                    name = existingDish.Category.Name
                },
                image = existingDish.ImageUrl,
                isActive = existingDish.Available,
                createdAt = existingDish.CreateDate,
                updatedAt = existingDish.UpdateDate
            };

        }

        public async Task<DishResponse> DeleteDish(Guid id)
        {
            var existingDish = await _query.GetDishByIdAsync(id);
            if (existingDish == null)
            {
                throw new NotFoundException("Plato no encontrado");
            }

            var ordersWithDish = await _orderQuery.GetOrdersWithDishAsync(id);

            foreach(var order in ordersWithDish)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.DishId == id && item.Status.Name != "Cancelled" && item.Status.Name != "Delivered")
                    {
                        throw new DishInUseException("No se puede eliminar el plato porque está incluido en órdenes activas");
                    }
                }
            }

            existingDish.Available = false;
            existingDish.UpdateDate = DateTime.UtcNow;

            await _command.UpdateDish(existingDish);
            return new DishResponse
            {
                id = existingDish.DishId,
                name = existingDish.Name,
                description = existingDish.Description,
                price = (double)existingDish.Price,
                Category = new GenericResponse
                {
                    id = existingDish.CategoryId,
                    name = existingDish.Category.Name
                },
                image = existingDish.ImageUrl,
                isActive = existingDish.Available,
                createdAt = existingDish.CreateDate,
                updatedAt = existingDish.UpdateDate
            };
        }

    }
}
