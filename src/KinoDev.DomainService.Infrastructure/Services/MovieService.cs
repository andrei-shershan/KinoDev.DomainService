using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.Shared.DtoModels;
using KinoDev.Shared.DtoModels.ShowingMovies;
using Microsoft.EntityFrameworkCore;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IMovieService
    {
        Task<MovieDto> GetByIdAsync(int id);
        Task<IEnumerable<MovieDto>> GetAllAsync();
        Task<MovieDto> CreateAsync(MovieDto movieDto);
        Task<MovieDto> UpdateAsync(int id, MovieDto movieDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date);
    }

    public class MovieService : IMovieService
    {
        private readonly KinoDevDbContext _dbContext;

        public MovieService(KinoDevDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<MovieDto> CreateAsync(MovieDto movieDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MovieDto>> GetAllAsync()
        {
            var movies = await _dbContext.Movies.ToListAsync();
            return movies.Select(x => x.ToDto());
        }

        public async Task<MovieDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Movie> GetMovies()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date)
        {
            var movies = await _dbContext.Movies.ToListAsync();
            var dbResults = await
                _dbContext
                    .Movies
                    .Join(_dbContext.ShowTimes, m => m.Id, st => st.MovieId, (m, st) => new { m, st })
                    .Join(_dbContext.Halls, x => x.st.HallId, h => h.Id, (x, h) => new { x.m, x.st, h })
                    .Where(x => DateOnly.FromDateTime(x.st.Time.Date) == DateOnly.FromDateTime(date))
                    .ToListAsync();

            var now = DateTime.UtcNow;

            var result = new List<ShowingMovie>();
            foreach (var group in dbResults.GroupBy(x => x.m.Id))
            {
                var item = group.FirstOrDefault();
                if (item != null)
                {
                    var moviesShowTimeDetails = group.Select(st => new MovieShowTimeDetails
                    {
                        Id = st.st.Id,
                        HallId = item.h.Id,
                        HallName = item.h.Name,
                        Time = st.st.Time,
                        Price = st.st.Price,
                        IsSellingAvailable = st.st.Time > now
                    }).OrderBy(st => st.Time);

                    result.Add(new ShowingMovie
                    {
                        Id = item.m.Id,
                        Name = item.m.Name,
                        Description = item.m.Description,
                        ReleaseDate = item.m.ReleaseDate,
                        Duration = item.m.Duration,
                        Url = item.m.Url,
                        MoviesShowTimeDetails = moviesShowTimeDetails
                    });
                }
            }

            return result;
        }

        public Task<MovieDto> UpdateAsync(int id, MovieDto movieDto)
        {
            throw new NotImplementedException();
        }
    }
}
