using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.Shared.DtoModels.Movies;

namespace KinoDev.DomainService.Infrastructure.Mappers
{
    public static class MovieMapper
    {
        public static MovieDto ToDto(this Movie movie)
        {
            if (movie == null)
            {
                return null;
            }
            
            return new MovieDto()
            {
                Description = movie.Description,
                Duration = movie.Duration,
                Id = movie.Id,
                Name = movie.Name,
                ReleaseDate = movie.ReleaseDate,
                Url = movie.Url,
            };
        }
    }
}
