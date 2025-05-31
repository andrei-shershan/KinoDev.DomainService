using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.ShowTimeControllerTests
{
    public class GetShowTimeDetailsTests : ShowTimesControllerBaseTests
    {
        [Fact]
        public async Task GetShowTimeDetails_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            var showTimeId = 1;
            _mockShowTimeService
                .Setup(s => s.GetDetailsByIdAsync(showTimeId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetShowTimeDetails(showTimeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockShowTimeService.Verify(s => s.GetDetailsByIdAsync(showTimeId), Times.Once);
        }

        [Fact]
        public async Task GetShowTimeDetails_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var showTimeId = 1;
            var expectedDetails = new ShowTimeDetailsDto
            {
                Id = showTimeId,
                Movie = new MovieDto()
                {
                    Id = 101,
                    Name = "Movie A",
                    Duration = 120,
                    Description = "An action-packed thriller.",
                    ReleaseDate = DateOnly.FromDateTime(new DateTime(2023, 1, 1)),
                    Url = "https://example.com/movie-a"
                },
                Hall = new HallDto()
                {
                    Id = 1,
                    Name = "Main Hall"
                },
                Time = new DateTime(2023, 10, 1, 18, 30, 0),
                Price = 10.00m
            };

            _mockShowTimeService
                .Setup(s => s.GetDetailsByIdAsync(showTimeId))
                .ReturnsAsync(expectedDetails);

            // Act
            var result = await _controller.GetShowTimeDetails(showTimeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equivalent(expectedDetails, okResult.Value);

            _mockShowTimeService.Verify(s => s.GetDetailsByIdAsync(showTimeId), Times.Once);
        }
    }
}