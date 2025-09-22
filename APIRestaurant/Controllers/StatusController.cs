using Application.Exceptions;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIRestaurant.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IServiceStatus _serviceStatus;
        public StatusController(IServiceStatus serviceStatus)
        {
            _serviceStatus = serviceStatus;
        }
        [HttpGet]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var result = await _serviceStatus.GetStatuses();
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
    }
}
