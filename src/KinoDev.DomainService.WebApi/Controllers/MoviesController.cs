using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KinoDev.Shared.Extensions;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;

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
            if (createdMovie == null)
            {
                return BadRequest("Failed to create movie. Please check the provided data.");
            }

            return CreatedAtAction(nameof(CreateMovie), createdMovie);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById([FromRoute] int id)
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

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok($"Test successful, {DateTime.UtcNow} UTC");
        }

        [HttpGet("test2")]
        public IActionResult Test2()
        {
            return Ok($"Test successful, {DateTime.UtcNow} UTC");
        }
    }
}
