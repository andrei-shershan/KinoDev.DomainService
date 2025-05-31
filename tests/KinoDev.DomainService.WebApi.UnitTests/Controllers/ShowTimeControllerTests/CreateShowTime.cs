using KinoDev.DomainService.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.ShowTimeControllerTests
{
    public class CreateShowTime : ShowTimesControllerBaseTests
    {
        [Fact]
        public async Task CreateShowTime_WhenModelIsValid_ReturnsBadRequest()
        {
            // Arrange
            CreateShowTimeRequest request = null;

            // Act
            var result = await _controller.CreateShowTime(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            _mockShowTimeService.Verify(s => s.CreateAsync(It.IsAny<CreateShowTimeRequest>()), Times.Never);
        }

        [Theory]
        [InlineData(true, typeof(CreatedResult))]
        [InlineData(false, typeof(BadRequestObjectResult))]
        public async Task CreateShowTime_WhenModelIsValid_ReturnsCreatedAtAction(bool creationResult, Type expectedResultType)
        {
            // Arrange
            var request = new CreateShowTimeRequest();

            _mockShowTimeService
                .Setup(s => s.CreateAsync(It.IsAny<CreateShowTimeRequest>()))
                .ReturnsAsync(creationResult);

            // Act
            var result = await _controller.CreateShowTime(request);

            // Assert
            Assert.IsType(expectedResultType, result);

            _mockShowTimeService.Verify(s => s.CreateAsync(It.IsAny<CreateShowTimeRequest>()), Times.Once);
        }
    }
}