using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Query
{
    public interface IDishQuery
    {
        Task<List<Dish>> GetListDishAsync(string? name, int? category, string? sortByPrice, bool? onlyActive);
        Task<Dish> GetDishByNameAsync(string nameDish);
        Task<Dish> GetDishByIdAsync(Guid id);

    }
}
