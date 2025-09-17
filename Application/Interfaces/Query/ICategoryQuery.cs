using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Query
{
    public interface ICategoryQuery
    {
        Task<List<Category>> GetListCategoryAsync();
        Task<Category> GetCategoryByIdAsync(int id);
    }
}
