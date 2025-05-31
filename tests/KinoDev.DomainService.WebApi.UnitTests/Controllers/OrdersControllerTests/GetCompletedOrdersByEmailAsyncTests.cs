using KinoDev.DomainService.WebApi.Models;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class GetCompletedOrdersByEmailAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task GetCompletedOrdersByEmailAsync_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            _mockOrderService
                .Setup(s => s.GetCompletedOrdersByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetCompletedOrdersByEmailAsync(new GetCompletedOrdersByEmailModel());

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockOrderService.Verify(s => s.GetCompletedOrdersByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrdersByEmailAsync_WhenEmptyData_ReturnsNotFound()
        {
            // Arrange
            var model = new GetCompletedOrdersByEmailModel
            {
                Email = "test@example.com"
            };

            _mockOrderService
                .Setup(s => s.GetCompletedOrdersByEmailAsync(model.Email))
                .ReturnsAsync(new List<OrderSummary>());

            // Act
            var result = await _controller.GetCompletedOrdersByEmailAsync(model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockOrderService.Verify(s => s.GetCompletedOrdersByEmailAsync(model.Email), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrdersByEmailAsync_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var model = new GetCompletedOrdersByEmailModel
            {
                Email = "test@example.com"
            };

            var expectedOrders = new List<OrderSummary>
            {
                new OrderSummary { Id = Guid.NewGuid(), Email = model.Email, Cost = 100 },
                new OrderSummary { Id = Guid.NewGuid(), Email = model.Email, Cost = 200 }
            };

            _mockOrderService
                .Setup(s => s.GetCompletedOrdersByEmailAsync(model.Email))
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetCompletedOrdersByEmailAsync(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderSummary>>(okResult.Value);
            
            Assert.Equal(expectedOrders, returnedOrders);
            _mockOrderService.Verify(s => s.GetCompletedOrdersByEmailAsync(model.Email), Times.Once);
        }
    }
}
