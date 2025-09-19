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
    public class DeliveryTypeQuery : IDeliveryTypeQuery
    {
        private readonly RestaurantDbContext _context;

        public DeliveryTypeQuery(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeliveryType>> GetListDeliveryTypeAsync()
        {
            return await _context.DeliveryType.ToListAsync();
        }
    }
}
