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
        private readonly IDateTimeService _dateTimeService;

        public MoviesController(IMovieService movieService, IDateTimeService dateTimeService)
        {
            _movieService = movieService;
            _dateTimeService = dateTimeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllAsync();
            if (movies.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(movies);
        }

        [HttpGet("showing")]
        public async Task<IActionResult> GetShowingMovies([FromQuery] DateTime date)
        {
            date = date.Date;
            if (date == default)
            {
                date = _dateTimeService.UtcNow();
            }

            var movies = await _movieService.GetShowingMoviesAsync(date);
            if (movies.IsNullOrEmptyCollection())
            {
                return NotFound();
            }

            return Ok(movies);
        }
    }
}
