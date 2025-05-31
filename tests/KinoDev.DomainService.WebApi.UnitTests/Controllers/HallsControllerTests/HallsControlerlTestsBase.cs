namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.HallsControllerTests
{
    using KinoDev.DomainService.Infrastructure.Services.Abstractions;
    using KinoDev.DomainService.WebApi.Controllers;
    using Moq;

    public class HallsControllerBaseTests
    {
        protected readonly Mock<IHallsService> _mockHallsService;
        protected readonly HallsController _controller;

        public HallsControllerBaseTests()
        {
            _mockHallsService = new Mock<IHallsService>();
            _controller = new HallsController(_mockHallsService.Object);
        }
    }
}