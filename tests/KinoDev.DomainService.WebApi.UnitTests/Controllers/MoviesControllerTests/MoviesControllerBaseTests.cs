using KinoDev.DomainService.Infrastructure.Services;
using KinoDev.DomainService.WebApi.Controllers;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.MoviesControllerTests
{
    public class MoviesControllerBaseTests
    {
        protected readonly Mock<IMovieService> _mockMovieService;
        protected readonly Mock<IDateTimeService> _mockTimeService;

        protected readonly MoviesController _controller;

        public MoviesControllerBaseTests()
        {
            _mockMovieService = new Mock<IMovieService>();
            _mockTimeService = new Mock<IDateTimeService>();

            _controller = new MoviesController(_mockMovieService.Object, _mockTimeService.Object);
        }        
    }
}
