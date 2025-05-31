using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.DomainService.WebApi.UnitTests.Controllers.ShowTimeControllerTests
{
    public class GetShowTimeDetailsUnitTests : ShowTimesControllerBaseTests
    {
        [Fact]
        public async Task GetShowTimeDetails_WhenNoData_ReturnsNotFound()
        {
            // Arrange
            var showTimeId = 1;
            _mockShowTimeService
                .Setup(s => s.GetDetailsByIdAsync(showTimeId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetShowTimeDetails(showTimeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockShowTimeService.Verify(s => s.GetDetailsByIdAsync(showTimeId), Times.Once);
        }

        [Fact]
        public async Task GetShowTimeDetails_WhenDataExists_ReturnsOkWithData()
        {
            // Arrange
            var showTimeId = 1;

            _mockShowTimeService
                .Setup(s => s.GetDetailsByIdAsync(showTimeId))
                .ReturnsAsync(new ShowTimeDetailsDto());

            // Act
            var result = await _controller.GetShowTimeDetails(showTimeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockShowTimeService.Verify(s => s.GetDetailsByIdAsync(showTimeId), Times.Once);
        }
    }
}