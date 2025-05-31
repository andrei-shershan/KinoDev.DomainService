using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowingMovies;

namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
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
}