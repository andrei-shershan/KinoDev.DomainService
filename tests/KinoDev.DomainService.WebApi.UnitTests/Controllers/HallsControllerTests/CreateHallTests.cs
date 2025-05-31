using KinoDev.DomainService.WebApi.Models;
using KinoDev.Shared.DtoModels.Hall;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.HallsControllerTests
{
    public class CreateHallTests : HallsControllerBaseTests
    {
        [Fact]
        public async Task CreateHallAsync_WhenHallDtoIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateHall(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid hall creation request.", badRequestResult.Value);

            _mockHallsService.Verify(s => s.CreateHallAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateHallAsync_WhenCreationFails_ReturnsBadRequest()
        {
            // Arrange
            _mockHallsService
                .Setup(s => s.CreateHallAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CreateHall(new CreateHallWithSeatsRequest()
            {
                Name = "Test Hall",
                RowsCount = 10,
                SeatsCount = 20
            });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Failed to create hall. Please check the provided data.", badRequestResult.Value);

            _mockHallsService.Verify(s => s.CreateHallAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateHallAsync_WhenCreationSucceeds_ReturnsCreatedAtAction()
        {
            // Arrange
            _mockHallsService
                .Setup(s => s.CreateHallAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new HallSummary());

            // Act
            var result = await _controller.CreateHall(new CreateHallWithSeatsRequest()
            {
                Name = "Test Hall",
                RowsCount = 10,
                SeatsCount = 20
            });

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;

            _mockHallsService.Verify(s => s.CreateHallAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}