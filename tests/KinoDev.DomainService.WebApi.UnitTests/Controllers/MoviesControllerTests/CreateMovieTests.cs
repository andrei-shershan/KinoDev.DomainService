using KinoDev.Shared.DtoModels.Movies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.MoviesControllerTests
{
    public class CreateMovieTests : MoviesControllerBaseTests
    {
        [Fact]
        public async Task CreateMovieAsync_WhenMovieDtoIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateMovie(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Movie data is required.", badRequestResult.Value);

            _mockMovieService.Verify(s => s.CreateAsync(It.IsAny<MovieDto>()), Times.Never);
        }

        [Fact]
        public async Task CreateMovieAsync_WhenCreationFails_ReturnsBadRequest()
        {
            // Arrange
            var movieDto = new MovieDto { Name = "Test Movie" };
            _mockMovieService
                .Setup(s => s.CreateAsync(movieDto))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CreateMovie(movieDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Failed to create movie. Please check the provided data.", badRequestResult.Value);

            _mockMovieService.Verify(s => s.CreateAsync(movieDto), Times.Once);
        }

        [Fact]
        public async Task CreateMovieAsync_WhenCreationSucceeds_ReturnsCreatedAtAction()
        {
            // Arrange
            var movieDto = new MovieDto { Name = "Test Movie" };
            var createdMovie = new MovieDto { Id = 1, Name = "Test Movie" };
            _mockMovieService
                .Setup(s => s.CreateAsync(movieDto))
                .ReturnsAsync(createdMovie);

            // Act
            var result = await _controller.CreateMovie(movieDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.Equal(nameof(_controller.CreateMovie), createdAtActionResult.ActionName);
            Assert.Equal(createdMovie, createdAtActionResult.Value);

            _mockMovieService.Verify(s => s.CreateAsync(movieDto), Times.Once);
        }
    }
}