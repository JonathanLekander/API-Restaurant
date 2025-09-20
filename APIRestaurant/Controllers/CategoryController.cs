using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIRestaurant.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceCategory _service;
        public CategoryController(IServiceCategory service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var result = await _service.GetCategories();
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrio un error inesperado.", details = ex.Message });
            }
        }
    }
}
