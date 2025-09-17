using Application.Interfaces.Command;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands
{
    public class DishCommand : IDishCommand
    {
        private readonly RestaurantDbContext _context;

        public DishCommand(RestaurantDbContext context)
        {
            _context = context;
        }
        public async Task CreateDish(Dish dish)
        {
            var ExistingDish = await _context.Dish
                .FirstOrDefaultAsync(d => d.Name == dish.Name);

            _context.Add(dish);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateDish(Dish dish)
        {
            _context.Dish.Update(dish);
            await _context.SaveChangesAsync();
        }

    }
}
