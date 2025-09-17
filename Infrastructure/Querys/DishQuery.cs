using Application.Interfaces.Query;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Querys
{
    public class DishQuery : IDishQuery
    {
        private readonly RestaurantDbContext _Context;

        public DishQuery(RestaurantDbContext context)
        {
            _Context = context;
        }
        public async Task<List<Dish>> GetListDishAsync(string? name, int? category, string? sortByPrice, bool? onlyActive)
        {
            var query = _Context.Dish
                .Include(d => d.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => d.Name.Contains(name));
            }

            if (category.HasValue)
            {
                query = query.Where(d => d.CategoryId == category);
            }

            if (onlyActive.HasValue)
            {
                query = query.Where(d => d.Available == onlyActive.Value);
            }

            switch (sortByPrice?.ToLower())
            {
                case "asc":
                    query = query.OrderBy(d => d.Price); break;
                case "desc":
                    query = query.OrderByDescending(d => d.Price); break;
            }
            return await query.ToListAsync();

        }
        public async Task<Dish> GetDishByNameAsync(string nameDish)
        {
            return await _Context.Dish
                    .Include(d => d.Category)
                    .FirstOrDefaultAsync(d => d.Name == nameDish);

        }
        public async Task<Dish> GetDishByIdAsync(Guid id)
        {
            return await _Context.Dish
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DishId == id);
        }
    }
}
