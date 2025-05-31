using KinoDev.Shared.DtoModels.Hall;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.HallsControllerTests
{
    public class GetHallByIdAsyncTests : HallsControllerBaseTests
    {
        [Fact]
        public async Task GetHallByIdAsync_WhenHallNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockHallsService.Setup(s => s.GetHallByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var id = 1;

            // Act
            var result = await _controller.GetHallByIdAsync(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal($"Hall with ID {id} not found.", notFoundResult.Value);

            _mockHallsService.Verify(s => s.GetHallByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetHallByIdAsync_WhenHallExists_ReturnsOkWithHall()
        {
            // Arrange
            var hall = new HallSummary { Id = 1, Name = "Test Hall" };
            _mockHallsService.Setup(s => s.GetHallByIdAsync(1)).ReturnsAsync(hall);

            // Act
            var result = await _controller.GetHallByIdAsync(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(hall, okResult.Value);

            _mockHallsService.Verify(s => s.GetHallByIdAsync(It.IsAny<int>()), Times.Once);
        }
    }
}