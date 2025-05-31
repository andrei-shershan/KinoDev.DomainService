using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Domain.DomainsModels;

namespace KinoDev.DomainService.Infrastructure.UnitTests.Mappers
{
    public class ShowTimeDetailsMapperTests
    {
        [Fact]
        public void ToDto_ShouldReturnShowTimeDetailsDto_WhenShowTimeIsNotNull()
        {
            // Arrange
            var movie = new Movie
            {
                Id = 1,
                Name = "Test Movie",
                Description = "Test Description",
                Duration = 120
            };

            var hall = new Hall
            {
                Id = 1,
                Name = "Hall A"
            };

            var showTime = new ShowTime
            {
                Id = 1,
                MovieId = movie.Id,
                HallId = hall.Id,
                Time = new DateTime(2025, 6, 1, 19, 0, 0),
                Price = 15.99m,
                Movie = movie,
                Hall = hall
            };

            // Act
            var result = showTime.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(showTime.Id, result.Id);
            Assert.Equal(showTime.Time, result.Time);
            Assert.Equal(showTime.Price, result.Price);

            // Check Movie property
            Assert.NotNull(result.Movie);
            Assert.Equal(movie.Id, result.Movie.Id);
            Assert.Equal(movie.Name, result.Movie.Name);

            // Check Hall property
            Assert.NotNull(result.Hall);
            Assert.Equal(hall.Id, result.Hall.Id);
            Assert.Equal(hall.Name, result.Hall.Name);
        }

        [Fact]
        public void ToDto_ShouldReturnNull_WhenShowTimeIsNull()
        {
            // Arrange
            ShowTime? showTime = null;

            // Act
            var result = ShowTimeDetailsMapper.ToDto(showTime);

            // Assert
            Assert.Null(result);
        }
    }
}
