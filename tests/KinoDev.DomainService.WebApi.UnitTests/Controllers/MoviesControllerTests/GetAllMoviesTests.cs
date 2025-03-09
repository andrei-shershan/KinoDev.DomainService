using KinoDev.Shared.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.MoviesControllerTests
{
    public class GetAllMoviesTests : MoviesControllerBaseTests
    {
        [Fact]
        public async Task GetAllMovies_ReturnsOkResult_WithListOfMovies()
        {
            // Arrange
            var movies = new List<MovieDto> { new MovieDto { Id = 1, Name = "Movie 1" }, new MovieDto { Id = 2, Name = "Movie 2" } };
            _mockMovieService.Setup(service => service.GetAllAsync()).ReturnsAsync(movies);

            // Act
            var result = await _controller.GetAllMovies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<MovieDto>>(okResult.Value);

            Assert.Equal(2, returnValue.Count);

            Assert.Contains(returnValue, x => x.Id == 1 && x.Name == "Movie 1");
            Assert.Contains(returnValue, x => x.Id == 2 && x.Name == "Movie 2");
        }

        [Theory]
        [MemberData(nameof(MoviesNullOrEmptyData))]
        public async Task GetAllMovies_ReturnsNotFound_WhenNoMoviesExist(List<MovieDto> movies)
        {
            // Arrange
            _mockMovieService.Setup(service => service.GetAllAsync()).ReturnsAsync(movies);

            // Act
            var result = await _controller.GetAllMovies();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        public static IEnumerable<object[]> MoviesNullOrEmptyData =>
            new List<object[]>
            {
                    new object[] { null },
                    new object[] { new List<MovieDto> { } }
            };
    }
}
