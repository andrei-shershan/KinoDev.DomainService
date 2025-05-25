using KinoDev.DomainService.Infrastructure.Services;
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
        [AllowAnonymous]
        public async Task<IActionResult> CreateHall([FromBody] HallDto hallDto)
        {
            if (hallDto == null)
            {
                return BadRequest("Hall data is required.");
            }

            var createdHall = await _hallsService.CreateHallAsync(hallDto);
            return CreatedAtAction(nameof(CreateHall), createdHall);
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetHallByIdAsync(int id)
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