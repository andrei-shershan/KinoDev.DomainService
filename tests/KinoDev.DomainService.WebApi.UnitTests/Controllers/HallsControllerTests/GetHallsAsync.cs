using KinoDev.Shared.DtoModels.Hall;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.HallsControllerTests
{
    public class GetHallsAsyncTests : HallsControllerBaseTests
    {
        [Fact]
        public async Task GetHallsAsync_WhenNoHallsFound_ReturnsNotFound()
        {
            // Arrange
            _mockHallsService.Setup(s => s.GetAllHallsAsync()).ReturnsAsync(new List<HallSummary>());

            // Act
            var result = await _controller.GetHallsAsync();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal("No halls found.", notFoundResult.Value);

            _mockHallsService.Verify(s => s.GetAllHallsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetHallsAsync_WhenHallsExist_ReturnsOkWithHalls()
        {
            // Arrange
            var halls = new List<HallSummary>
            {
                new HallSummary { Id = 1, Name = "Hall 1" },
                new HallSummary { Id = 2, Name = "Hall 2" }
            };
            _mockHallsService.Setup(s => s.GetAllHallsAsync()).ReturnsAsync(halls);

            // Act
            var result = await _controller.GetHallsAsync();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(halls, okResult.Value);

            _mockHallsService.Verify(s => s.GetAllHallsAsync(), Times.Once);
        }
    }
}