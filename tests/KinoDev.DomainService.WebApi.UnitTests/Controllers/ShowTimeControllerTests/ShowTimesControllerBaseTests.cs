using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.DomainService.WebApi.Controllers;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers
{
    public class ShowTimesControllerBaseTests
    {
        protected readonly Mock<IShowTimesService> _mockShowTimeService;
        protected readonly ShowTimesController _controller;

        public ShowTimesControllerBaseTests()
        {
            _mockShowTimeService = new Mock<IShowTimesService>();
            _controller = new ShowTimesController(_mockShowTimeService.Object);
        }
    }
}