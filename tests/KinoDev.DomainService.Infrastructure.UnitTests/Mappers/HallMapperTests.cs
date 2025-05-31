using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.Shared.DtoModels.Hall;

namespace KinoDev.DomainService.Infrastructure.UnitTests.Mappers
{
    public class HallMapperTests
    {
        [Fact]
        public void ToDto_ShouldReturnHallDto_WhenHallIsNotNull()
        {
            // Arrange
            var hall = new Hall
            {
                Id = 1,
                Name = "Hall A"
            };

            // Act
            var result = hall.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(hall.Id, result.Id);
            Assert.Equal(hall.Name, result.Name);
        }

        [Fact]
        public void ToDto_ShouldReturnNull_WhenHallIsNull()
        {
            // Arrange
            Hall? hall = null;

            // Act
            var result = HallMapper.ToDto(hall);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ToDomainModel_ShouldReturnHall_WhenHallDtoIsNotNull()
        {
            // Arrange
            var hallDto = new HallDto
            {
                Id = 1,
                Name = "Hall A"
            };

            // Act
            var result = hallDto.ToDomainModel();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(hallDto.Id, result.Id);
            Assert.Equal(hallDto.Name, result.Name);
        }

        [Fact]
        public void ToDomainModel_ShouldReturnNull_WhenHallDtoIsNull()
        {
            // Arrange
            HallDto? hallDto = null;

            // Act
            var result = HallMapper.ToDomainModel(hallDto);

            // Assert
            Assert.Null(result);
        }
    }
}
