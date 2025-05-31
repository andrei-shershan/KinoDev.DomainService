using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.Orders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class CreateOrderAsyncTests : OrdersControllerBaseTests
    {
        [Fact]
        public async Task CreateOrderAsync_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var orderModel = new CreateOrderModel
            {
                ShowTimeId = 1,
                SelectedSeatIds = new List<int> { 1, 2 },
                Email = "test@example.com"
            };

            _mockOrderService
                .Setup(s => s.CreateOrderAsync(It.IsAny<CreateOrderModel>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.CreateOrderAsync(orderModel);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockOrderService.Verify(s => s.CreateOrderAsync(It.IsAny<CreateOrderModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WhenServiceSucceeds_ReturnsOkWithData()
        {
            // Arrange
            var orderModel = new CreateOrderModel
            {
                ShowTimeId = 1,
                SelectedSeatIds = new List<int> { 1, 2 },
                Email = "test@example.com"
            };

            var createdOrder = new OrderSummary
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Cost = 100,
                Email = "test@example.com"
            };

            _mockOrderService
                .Setup(s => s.CreateOrderAsync(orderModel))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _controller.CreateOrderAsync(orderModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderSummary>(okResult.Value);
            
            Assert.Equal(createdOrder, returnedOrder);
            _mockOrderService.Verify(s => s.CreateOrderAsync(orderModel), Times.Once);
        }
    }
}
