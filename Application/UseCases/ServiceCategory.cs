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
            var categoryResponse = new List<CategoryResponse>();

            foreach (var category in categories)
            {
                var categoryDto = new CategoryResponse
                {
                    id = category.Id,
                    name = category.Name,
                    description = category.Description
                };
                categoryResponse.Add(categoryDto);
            }
            return categoryResponse;
        }
    }
}
