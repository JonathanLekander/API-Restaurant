using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIRestaurant.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeliveryTypeController : ControllerBase
    {
        private readonly IServiceDeliveryType _serviceDeliveryType;

        public DeliveryTypeController(IServiceDeliveryType serviceDeliveryType)
        {
            _serviceDeliveryType = serviceDeliveryType;
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryTypes()
        {
            try
            {
                var result = await _serviceDeliveryType.GetDeliveryTypes();
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrio un error inesperado.", details = ex.Message });
            }
        }
    }
}
