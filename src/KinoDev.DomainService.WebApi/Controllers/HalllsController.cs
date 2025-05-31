using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.DomainService.WebApi.Models;
using KinoDev.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HallsController : ControllerBase
    {
        private readonly IHallsService _hallsService;

        public HallsController(IHallsService hallsService)
        {
            _hallsService = hallsService ?? throw new ArgumentNullException(nameof(hallsService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateHall([FromBody] CreateHallWithSeatsRequest request)
        {
            // TODO: Add validation logic for the request model
            if (
                request == null
                || string.IsNullOrWhiteSpace(request.Name)
                || request.RowsCount <= 0
                || request.SeatsCount <= 0
            )
            {
                return BadRequest("Invalid hall creation request.");
            }

            var createdHall = await _hallsService.CreateHallAsync(request.Name, request.RowsCount, request.SeatsCount);
            if (createdHall == null)
            {
                return BadRequest("Failed to create hall. Please check the provided data.");
            }

            return CreatedAtAction(nameof(CreateHall), createdHall);
        }

        [HttpGet]
        public async Task<IActionResult> GetHallsAsync()
        {
            var halls = await _hallsService.GetAllHallsAsync();
            if (halls.IsNullOrEmptyCollection())
            {
                return NotFound("No halls found.");
            }

            return Ok(halls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHallByIdAsync([FromRoute] int id)
        {
            var hall = await _hallsService.GetHallByIdAsync(id);
            if (hall == null)
            {
                return NotFound($"Hall with ID {id} not found.");
            }

            return Ok(hall);
        }
    }
}