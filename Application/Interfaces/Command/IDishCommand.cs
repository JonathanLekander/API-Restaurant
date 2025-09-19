using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Command
{
    public interface IDishCommand
    {
        Task CreateDish(Dish dish);
        Task UpdateDish(Dish dish);
        // Task DeleteDish(Guid id);


    }
}
