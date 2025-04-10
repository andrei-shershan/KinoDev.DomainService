using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ShowTimesController : ControllerBase
    {
        private readonly IShowTimeService _showTimeService;

        public ShowTimesController(IShowTimeService showTimeService)
        {
            _showTimeService = showTimeService;
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetShowTimeDetails([FromRoute] int id)
        {
            var result = await _showTimeService.GetDetailsByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("seats/{id}")]
        public async Task<IActionResult> GetShowTimeSeats([FromRoute] int id)
        {
            var result = await _showTimeService.GetShowTimeSeatsAsync(id);
            if (result == null)
            {
                return NotFound("");
            }

            return Ok(result);
        }
    }
}