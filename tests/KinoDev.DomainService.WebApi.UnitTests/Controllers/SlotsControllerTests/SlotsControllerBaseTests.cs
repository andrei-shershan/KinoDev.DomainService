using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.DomainService.WebApi.Controllers;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.SlotsControllerTests
{
    public class SlotsControllerBaseTests
    {
        protected readonly Mock<ISlotService> _mockSlotService;
        protected readonly SlotsController _controller;

        public SlotsControllerBaseTests()
        {
            _mockSlotService = new Mock<ISlotService>();
            _controller = new SlotsController(_mockSlotService.Object);
        }
    }
}
