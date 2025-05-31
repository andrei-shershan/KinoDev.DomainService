using KinoDev.Shared.DtoModels.Movies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.MoviesControllerTests
{
    public class GetMovieByIdTests : MoviesControllerBaseTests
    {
        [Fact]
        public async Task GetMovieByIdAsync_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            var movieId = 1;
            _mockMovieService
                .Setup(s => s.GetByIdAsync(movieId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetMovieById(movieId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockMovieService.Verify(s => s.GetByIdAsync(movieId), Times.Once);
        }

        [Fact]
        public async Task GetMovieByIdAsync_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var movieId = 1;
            var expectedMovie = new MovieDto
            {
                Id = movieId,
                Name = "Test Movie",
                Duration = 120,
                Description = "A test movie description.",
                ReleaseDate = DateOnly.FromDateTime(new DateTime(2023, 1, 1)),
                Url = "https://example.com/test-movie",
            };

            _mockMovieService
                .Setup(s => s.GetByIdAsync(movieId))
                .ReturnsAsync(() => expectedMovie);

            // Act
            var result = await _controller.GetMovieById(movieId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualMovie = Assert.IsType<MovieDto>(okResult.Value);
            Assert.Equivalent(expectedMovie, actualMovie);

            _mockMovieService.Verify(s => s.GetByIdAsync(movieId), Times.Once);
        }
    }
}