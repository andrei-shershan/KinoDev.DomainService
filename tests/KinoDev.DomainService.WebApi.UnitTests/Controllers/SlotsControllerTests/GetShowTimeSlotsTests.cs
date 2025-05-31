using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.SlotsControllerTests
{
    public class GetShowTimeSlotsTests : SlotsControllerBaseTests
    {
        [Fact]
        public async Task GetShowTimeSlotsAsync_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            _mockSlotService
                .Setup(s => s.GetShowTimeSlotsAsync(date))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetShowTimeSlotsAsync(date);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockSlotService.Verify(s => s.GetShowTimeSlotsAsync(date), Times.Once);
        }

        [Fact]
        public async Task GetShowTimeSlotsAsync_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            var expectedSlots =
                new ShowTimeForDateDto
                {
                    Date = date,
                    HallWithMovies = new List<HallWithMoviesDto>
                    {
                        new HallWithMoviesDto
                        {
                            Hall = new HallDto
                            {
                                Id = 1,
                                Name = "Main Hall",
                            },
                            Movies = new List<MovieWithShowTime>
                            {
                                new MovieWithShowTime
                                {
                                    Id = 101,
                                    Name = "Movie A",
                                    Duration = 120,
                                    Description = "An action-packed thriller.",
                                    ReleaseDate = DateOnly.FromDateTime(new DateTime(2023, 1, 1)),
                                    Url = "https://example.com/movie-a",
                                },
                            }
                        }
                    }
                };

            _mockSlotService
                .Setup(s => s.GetShowTimeSlotsAsync(date))
                .ReturnsAsync(() => expectedSlots);

            // Act
            var result = await _controller.GetShowTimeSlotsAsync(date);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualSlots = Assert.IsAssignableFrom<ShowTimeForDateDto>(okResult.Value);
            Assert.Equivalent(expectedSlots, actualSlots);

            _mockSlotService.Verify(s => s.GetShowTimeSlotsAsync(date), Times.Once);
        }
    }
}
