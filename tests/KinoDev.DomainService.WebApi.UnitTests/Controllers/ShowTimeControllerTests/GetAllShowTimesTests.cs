using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.ShowTimeControllerTests
{
    public class GetAllShowTimesTests : ShowTimesControllerBaseTests
    {
        private DateTime _startDate = new DateTime(2023, 10, 1);
        private DateTime _endDate = new DateTime(2023, 10, 31);

        [Fact]
        public async Task GetAllShowTimes_WhenNoData_ReturnsEmptyList()
        {
            // Arrange
            _mockShowTimeService
                .Setup(s => s.GetAllAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetAllShowTimes(_startDate, _endDate);

            // Assert
            var okResult = Assert.IsType<NotFoundResult>(result);
            _mockShowTimeService.Verify(s => s.GetAllAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetAllShowTimes_WhenDataExists_ReturnsListOfShowTimes()
        {
            // Arrange
            _mockShowTimeService
                .Setup(s => s.GetAllAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new List<ShowTimeDetailsDto>()
                {
                    new ShowTimeDetailsDto()
                });

            // Act
            var result = await _controller.GetAllShowTimes(_startDate, _endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockShowTimeService.Verify(s => s.GetAllAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
}