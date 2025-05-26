using KinoDev.DomainService.Infrastructure.Services;
using KinoDev.DomainService.WebApi.Models;
using KinoDev.Shared.DtoModels.Hall;
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
                return StatusCode(500, "An error occurred while creating the hall.");
            }
            return CreatedAtAction(nameof(CreateHall), createdHall);
        }

        [HttpGet]
        public async Task<IActionResult> GetHallsAsync()
        {
            var halls = await _hallsService.GetAllHallsAsync();
            if (halls == null || !halls.Any())
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