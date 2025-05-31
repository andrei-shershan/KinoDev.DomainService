using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.DomainService.WebApi.Controllers;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.OrdersControllerTests
{
    public class OrdersControllerBaseTests
    {
        protected readonly Mock<IOrderService> _mockOrderService;
        protected readonly OrdersController _controller;

        public OrdersControllerBaseTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockOrderService.Object);
        }
    }
}
