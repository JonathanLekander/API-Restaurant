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
    public class StatusQuery : IStatusQuery
    {
        private readonly RestaurantDbContext _context;

        public StatusQuery(RestaurantDbContext context)
        {
            _context = context;
        }
        public async Task<List<Status>> GetListStatusAsync()
        {
            return await _context.Status.ToListAsync();
        }

    }
}
