using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class DeleteOrderAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task DeleteOrderAsync_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mockOrderService
                .Setup(s => s.DeleteOrderAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteOrderAsync(orderId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockOrderService.Verify(s => s.DeleteOrderAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenServiceSucceeds_ReturnsOk()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mockOrderService
                .Setup(s => s.DeleteOrderAsync(orderId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrderAsync(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<bool>(okResult.Value);
            
            Assert.True(returnedValue);
            _mockOrderService.Verify(s => s.DeleteOrderAsync(orderId), Times.Once);
        }
    }
}
