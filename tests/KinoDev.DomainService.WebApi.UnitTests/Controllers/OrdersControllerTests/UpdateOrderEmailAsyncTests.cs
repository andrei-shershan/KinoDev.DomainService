using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class UpdateOrderEmailAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task UpdateOrderEmailAsync_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var email = "new-email@example.com";

            _mockOrderService
                .Setup(s => s.UpdateOrderEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.UpdateOrderEmailAsync(orderId, email);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockOrderService.Verify(s => s.UpdateOrderEmailAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderEmailAsync_WhenServiceSucceeds_ReturnsOkWithData()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var email = "new-email@example.com";

            var updatedOrder = new OrderDto
            {
                Id = orderId,
                Email = email
            };

            _mockOrderService
                .Setup(s => s.UpdateOrderEmailAsync(orderId, email))
                .ReturnsAsync(updatedOrder);

            // Act
            var result = await _controller.UpdateOrderEmailAsync(orderId, email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
            
            Assert.Equal(updatedOrder, returnedOrder);
            Assert.Equal(email, returnedOrder.Email);
            _mockOrderService.Verify(s => s.UpdateOrderEmailAsync(orderId, email), Times.Once);
        }
    }
}
