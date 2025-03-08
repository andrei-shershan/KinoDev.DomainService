using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.Shared.DtoModels;
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

        public Task<MovieDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Movie> GetMovies()
        {
            throw new NotImplementedException();
        }

        public Task<MovieDto> UpdateAsync(int id, MovieDto movieDto)
        {
            throw new NotImplementedException();
        }
    }
}
