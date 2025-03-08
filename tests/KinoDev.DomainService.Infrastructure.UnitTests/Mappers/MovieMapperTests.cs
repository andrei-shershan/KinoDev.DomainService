using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Domain.DomainsModels;

namespace KinoDev.DomainService.Infrastructure.UnitTests.Mappers
{
    public class MovieMapperTests
    {
        [Fact]
        public void ToDto_ShouldReturnMovieDto_WhenMovieIsNotNull()
        {
            // Arrange
            var movie = new Movie
            {
                Description = "A great movie",
                Duration = 120,
                Id = 1,
                Name = "Movie Name",
                ReleaseDate = new DateOnly(2023, 1, 1),
                Url = "http://example.com"
            };

            // Act
            var result = movie.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movie.Description, result.Description);
            Assert.Equal(movie.Duration, result.Duration);
            Assert.Equal(movie.Id, result.Id);
            Assert.Equal(movie.Name, result.Name);
            Assert.Equal(movie.ReleaseDate, result.ReleaseDate);
            Assert.Equal(movie.Url, result.Url);
        }

        [Fact]
        public void ToDto_ShouldReturnNull_WhenMovieIsNull()
        {
            // Arrange
            Movie movie = null;

            // Act
            var result = movie.ToDto();

            // Assert
            Assert.Null(result);
        }
    }
}
