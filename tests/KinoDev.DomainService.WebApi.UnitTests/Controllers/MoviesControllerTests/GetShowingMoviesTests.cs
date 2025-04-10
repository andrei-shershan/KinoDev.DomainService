using KinoDev.Shared.DtoModels.ShowingMovies;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.MoviesControllerTests
{
    public class GetShowingMoviesTests : MoviesControllerBaseTests
    {
        [Theory]
        [InlineData("2021-01-01", "2021-01-01")]
        [InlineData("0001-01-01", "2022-02-02")]
        public async Task GetShowingMovies_ReturnsOkResult_WhenMoviesExist(DateTime date, DateTime expectedDate)
        {
            _mockTimeService.Setup(service => service.UtcNow()).Returns(expectedDate);

            // Arrange
            var showingMovies = new List<ShowingMovie>()
            {
                new ShowingMovie()
                {
                    Id = 1,
                    Name = "Movie 1",
                    MoviesShowTimeDetails = new List<MovieShowTimeDetails>()
                    {
                        new MovieShowTimeDetails()
                        {
                            HallId = 1,
                            HallName = "Hall 1",
                            IsSellingAvailable = true,
                            Price = 10,
                            Time = DateTime.UtcNow
                        }
                    }
                }
            };

            _mockMovieService.Setup(service => service.GetShowingMoviesAsync(It.IsAny<DateTime>())).ReturnsAsync(showingMovies);

            // Act
            var result = await _controller.GetShowingMovies(date);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ShowingMovie>>(okResult.Value);
            Assert.Equal(showingMovies, returnValue);

            Assert.Contains(returnValue, x => x.Id == 1 && x.Name == "Movie 1" && x.MoviesShowTimeDetails.FirstOrDefault(st => st.HallId == 1) != null);

            _mockMovieService.Verify(x => x.GetShowingMoviesAsync(It.Is<DateTime>(d => d == expectedDate)), Times.Once);
        }

        [Theory]
        [MemberData(nameof(ShowingMoviesNullOrEmptyData))]
        public async Task GetShowingMovies_ReturnsNotFoundResult_WhenNoMoviesExist(IEnumerable<ShowingMovie> showingMovies)
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            _mockMovieService.Setup(service => service.GetShowingMoviesAsync(date)).ReturnsAsync(showingMovies);

            // Act
            var result = await _controller.GetShowingMovies(date);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        public static IEnumerable<object[]> ShowingMoviesNullOrEmptyData =>
            new List<object[]>
            {
                    new object[] { null },
                    new object[] { new List<ShowingMovie> { } }
            };
    }
}
