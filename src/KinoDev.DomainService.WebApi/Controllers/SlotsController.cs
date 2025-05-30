using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet("{date:datetime}")]
        public async Task<IActionResult> GetShowTimeSlotsAsync([FromRoute] DateTime date)
        {
            var result = await _slotService.GetShowTimeSlotsAsync(date);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}