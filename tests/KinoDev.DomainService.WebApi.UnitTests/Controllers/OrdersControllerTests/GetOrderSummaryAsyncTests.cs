using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class GetOrderSummaryAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task GetOrderSummaryAsync_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockOrderService
                .Setup(s => s.GetOrderAsync(orderId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetOrderSummaryAsync(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockOrderService.Verify(s => s.GetOrderAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task GetOrderSummaryAsync_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedOrder = new OrderSummary
            {
                Id = orderId,
                CreatedAt = DateTime.Now,
                Cost = 100,
                Email = "test@example.com"
            };

            _mockOrderService
                .Setup(s => s.GetOrderAsync(orderId))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.GetOrderSummaryAsync(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderSummary>(okResult.Value);
            
            Assert.Equal(expectedOrder, returnedOrder);
            _mockOrderService.Verify(s => s.GetOrderAsync(orderId), Times.Once);
        }
    }
}
