using Application.Interfaces.Command;
using Application.Interfaces.Query;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Querys
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly RestaurantDbContext _context;

        public CategoryQuery(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetListCategoryAsync()
        {
            var CategoryList = new List<Category>();

            return await _context.Category.ToListAsync();
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Category
                .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
