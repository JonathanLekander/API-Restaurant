using Application.DTOs.Request;
using Application.Exceptions;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APIRestaurant.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IServiceOrder _serviceOrder;

        public OrderController(IServiceOrder serviceOrder)
        {
            _serviceOrder = serviceOrder;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest request)
        {
            try
            {
                var result = await _serviceOrder.CreateOrder(request);
                return new JsonResult(result) { StatusCode = 201 };
            }

            catch (InvalidParameterException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrio un error inesperado.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(DateTime? from, DateTime? to, int? status)
        {
            try
            {
                var result = await _serviceOrder.GetOrders(from, to, status);
                return new JsonResult(result) { StatusCode = 200 };
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(long id, OrderUpdateRequest request)
        {
            try
            {
                var result = await _serviceOrder.UpdateOrder(id, request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(long id)
        {
            try
            {
                var order = await _serviceOrder.GetOrderById(id);
                return new JsonResult(order) { StatusCode = 200 };
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrio un error inesperado.", details = ex.Message });
            }
        }

        [HttpPut("{orderId}/items/{itemId}")]
        public async Task<IActionResult> UpdateOrderItem(long orderId, long itemId,OrderItemUpdateRequest request)
        {
            try
            {
                var result = await _serviceOrder.UpdateOrderItem(orderId, itemId, request);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
    }
}