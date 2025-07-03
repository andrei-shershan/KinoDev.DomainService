using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UpController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Up()
        {
            return Ok($"DomainService ::: Up at {DateTime.UtcNow}");
        }
    }
}