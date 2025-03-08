using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace KinoDev.DomainService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllAsync();
            if (!movies.IsNullOrEmpty())
            {
                return Ok(movies);
            }

            return NotFound();
        }
    }
}
