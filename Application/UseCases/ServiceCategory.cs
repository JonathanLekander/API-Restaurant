using Application.DTOs.Response;
using Application.Interfaces.Query;
using Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class ServiceCategory : IServiceCategory
    {
        private readonly ICategoryQuery _categoryQuery;
        public ServiceCategory(ICategoryQuery categoryQuery)
        {
            _categoryQuery = categoryQuery;
        }
        public async Task<List<CategoryResponse>> GetCategories()
        {
            var categories = await _categoryQuery.GetListCategoryAsync();
            var sortedCategories = categories.OrderBy(c => c.Order).ToList();

            return sortedCategories.Select(c => new CategoryResponse
            {
                id = c.Id,
                name = c.Name,
                description = c.Description,
                order = c.Order
            }).ToList();
        }
    }
}
