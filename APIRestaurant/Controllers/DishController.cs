using Application.DTOs.Request;
using Application.Exceptions;
using Application.Interfaces.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIRestaurant.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IServiceDish _service;

        public DishController(IServiceDish service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDish(DishRequest request)
        {
            try
            {
                var result = await _service.CreateDish(request);
                return new JsonResult(result) { StatusCode = 201 };

            }
            catch (NameException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (PriceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidParameterException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrio un error inesperado.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDishes(string? name, int? category, string? sortByPrice, bool? onlyActive)
        {
            try
            {
                var result = await _service.GetDishes(name, category, sortByPrice, onlyActive);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (InvalidParameterException ex)
            {
                return BadRequest(new { message = ex.Message }); ;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrio un error inesperado.", details = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDishesById(string? id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest(new { message = "Formato de ID inválido" });
            }

            try
            {
                var result = await _service.GetDishById(guid);
                return new JsonResult(result) { StatusCode = 200 };
            }

            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404 
            }
            catch (AvailableException ex)
            {
                return Conflict(new { message = ex.Message }); // 409
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDish(Guid id, DishUpdateRequest request)
        {
            try
            {
                var result = await _service.UpdateDish(id, request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (NameException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (PriceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidParameterException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(Guid id)
        {
            try
            {
                var result = await _service.DeleteDish(id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DishInUseException ex)  
            {
                return Conflict(new { message = ex.Message });
            }
            catch (AvailableException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", details = ex.Message });
            }
        }
    }
}
