using KinoDev.DomainService.Domain.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly KinoDevDbContext _context;

        public TestController(KinoDevDbContext context)
        {
            _context = context;
        }

        [HttpGet("hello")]
        [Authorize]
        public async Task<IActionResult> Hello()
        {
            var halls = await _context.Halls.ToListAsync();

            return Ok($"Domain service response from DB: {JsonConvert.SerializeObject(halls)}");
        }

        [HttpGet("auth")]
        [Authorize]
        public async Task<IActionResult> HeloAutj()
        {
            var halls = await _context.Halls.ToListAsync();

            return Ok($"AUTHENTICATED::: Domain service response from DB: {JsonConvert.SerializeObject(halls)}");
        }
    }
}
