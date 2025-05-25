using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KinoDev.Shared.Extensions;
using KinoDev.Shared.DtoModels.Movies;

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

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieDto movieDto)
        {
            // Validate the incoming data
            if (movieDto == null)
            {
                return BadRequest("Movie data is required.");
            }

            var createdMovie = await _movieService.CreateAsync(movieDto);

            return CreatedAtAction(nameof(CreateMovie), createdMovie);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
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
