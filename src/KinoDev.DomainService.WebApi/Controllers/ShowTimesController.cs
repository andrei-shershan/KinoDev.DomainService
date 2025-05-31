using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShowTimesController : ControllerBase
    {
        private readonly IShowTimesService _showTimeService;

        public ShowTimesController(IShowTimesService showTimeService)
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
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> GetAllShowTimes([FromRoute] DateTime startDate, [FromRoute] DateTime endDate)
        {
            var result = await _showTimeService.GetAllAsync(startDate, endDate);
            if (result.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShowTime([FromBody] CreateShowTimeRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid show time data.");
            }

            var result = await _showTimeService.CreateAsync(request);
            if (!result)
            {
                return BadRequest("Failed to create show time.");
            }

            return Created();
        }
    }
}