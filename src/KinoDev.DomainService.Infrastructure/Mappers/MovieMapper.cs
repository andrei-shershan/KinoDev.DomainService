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
        
        public static Movie ToDomainModel(this MovieDto movieDto)
        {
            if (movieDto == null)
            {
                return null;
            }

            return new Movie()
            {
                Id = movieDto.Id,
                Name = movieDto.Name,
                Description = movieDto.Description,
                ReleaseDate = movieDto.ReleaseDate,
                Duration = movieDto.Duration,
                Url = movieDto.Url
            };
        }
    }
}
