using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class CompleteOrderAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task CompleteOrderAsync_WhenServiceFails_ReturnsNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mockOrderService
                .Setup(s => s.CompleteOrderAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CompleteOrderAsync(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockOrderService.Verify(s => s.CompleteOrderAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task CompleteOrderAsync_WhenServiceSucceeds_ReturnsOkWithData()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var completedOrder = new OrderDto
            {
                Id = orderId,
                State = KinoDev.Shared.Enums.OrderState.Completed,
                CompletedAt = DateTime.Now
            };

            _mockOrderService
                .Setup(s => s.CompleteOrderAsync(orderId))
                .ReturnsAsync(completedOrder);

            // Act
            var result = await _controller.CompleteOrderAsync(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
            
            Assert.Equal(completedOrder, returnedOrder);
            Assert.Equal(KinoDev.Shared.Enums.OrderState.Completed, returnedOrder.State);
            Assert.NotNull(returnedOrder.CompletedAt);
            _mockOrderService.Verify(s => s.CompleteOrderAsync(orderId), Times.Once);
        }
    }
}
