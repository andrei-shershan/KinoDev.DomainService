using KinoDev.DomainService.WebApi.Models;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class GetCompletedOrdersAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task GetCompletedOrdersAsync_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            _mockOrderService
                .Setup(s => s.GetCompletedOrdersAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetCompletedOrdersAsync(new GetCompletedOrdersModel());

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockOrderService.Verify(s => s.GetCompletedOrdersAsync(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrdersAsync_WhenEmptyData_ReturnsNotFound()
        {
            // Arrange
            var model = new GetCompletedOrdersModel
            {
                OrderIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockOrderService
                .Setup(s => s.GetCompletedOrdersAsync(model.OrderIds))
                .ReturnsAsync(new List<OrderSummary>());

            // Act
            var result = await _controller.GetCompletedOrdersAsync(model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockOrderService.Verify(s => s.GetCompletedOrdersAsync(model.OrderIds), Times.Once);
        }

        [Fact]
        public async Task GetCompletedOrdersAsync_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var model = new GetCompletedOrdersModel
            {
                OrderIds = new List<Guid> { orderId1, orderId2 }
            };

            var expectedOrders = new List<OrderSummary>
            {
                new OrderSummary { Id = orderId1, Cost = 100 },
                new OrderSummary { Id = orderId2, Cost = 200 }
            };

            _mockOrderService
                .Setup(s => s.GetCompletedOrdersAsync(model.OrderIds))
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetCompletedOrdersAsync(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderSummary>>(okResult.Value);
            
            Assert.Equal(expectedOrders, returnedOrders);
            _mockOrderService.Verify(s => s.GetCompletedOrdersAsync(model.OrderIds), Times.Once);
        }
    }
}
