using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IServiceDish
    {
        Task<DishResponse> CreateDish(DishRequest request);
        Task<DishResponse> UpdateDish(Guid dishId, DishUpdateRequest request);
        Task<List<DishResponse>> GetDishes(string? name, int? category, string sortByPrice, bool? onlyActive);
        Task <DishResponse> GetDishById(Guid id);
        Task <DishResponse> DeleteDish(Guid id);

    }
}
