using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KinoDev.Shared.Extensions;

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
            if (!movies.IsNullOrEmptyCollection())
            {
                return Ok(movies);
            }

            return NotFound();
        }

        [HttpGet("showing")]
        public async Task<IActionResult> GetShowingMovies([FromQuery] DateTime date)
        {
            date = date.Date;
            if (date == default)
            {
                date = DateTime.UtcNow.Date;
            }

            var movies = await _movieService.GetShowingMovies(date);
            if (!movies.IsNullOrEmptyCollection())
            {
                return Ok(movies);
            }

            return NotFound();
        }
    }
}
