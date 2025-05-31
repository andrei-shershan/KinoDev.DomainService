using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.Enums;

namespace KinoDev.DomainService.Infrastructure.UnitTests.Mappers
{
    public class OrderMapperTests
    {
        [Fact]
        public void ToDto_ShouldReturnOrderDto_WhenOrderIsNotNull()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CreatedAt = new DateTime(2025, 5, 15),
                CompletedAt = new DateTime(2025, 5, 16),
                Cost = 125.50m,
                State = OrderState.Completed,
                Email = "test@example.com",
                EmailSent = true,
                UserId = Guid.NewGuid(),
                FileUrl = "https://example.com/tickets/123.pdf"
            };

            // Act
            var result = order.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(order.CreatedAt, result.CreatedAt);
            Assert.Equal(order.CompletedAt, result.CompletedAt);
            Assert.Equal(order.Cost, result.Cost);
            Assert.Equal(order.State, result.State);
            Assert.Equal(order.Email, result.Email);
            Assert.Equal(order.EmailSent, result.EmailSent);
            Assert.Equal(order.UserId, result.UserId);
            Assert.Equal(order.FileUrl, result.FileUrl);
        }

        [Fact]
        public void ToDto_ShouldReturnNull_WhenOrderIsNull()
        {
            // Arrange
            Order? order = null;

            // Act
            var result = OrderMapper.ToDto(order);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ToDomainModel_ShouldReturnOrder_WhenOrderDtoIsNotNull()
        {
            // Arrange
            var orderDto = new OrderDto
            {
                Id = Guid.NewGuid(),
                CreatedAt = new DateTime(2025, 5, 15),
                CompletedAt = new DateTime(2025, 5, 16),
                Cost = 125.50m,
                State = OrderState.Completed,
                Email = "test@example.com",
                EmailSent = true,
                UserId = Guid.NewGuid(),
                FileUrl = "https://example.com/tickets/123.pdf"
            };

            // Act
            var result = orderDto.ToDomainModel();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDto.Id, result.Id);
            Assert.Equal(orderDto.CreatedAt, result.CreatedAt);
            Assert.Equal(orderDto.CompletedAt, result.CompletedAt);
            Assert.Equal(orderDto.Cost, result.Cost);
            Assert.Equal(orderDto.State, result.State);
            Assert.Equal(orderDto.Email, result.Email);
            Assert.Equal(orderDto.EmailSent, result.EmailSent);
            Assert.Equal(orderDto.UserId, result.UserId);
            Assert.Equal(orderDto.FileUrl, result.FileUrl);
        }

        [Fact]
        public void ToDomainModel_ShouldReturnNull_WhenOrderDtoIsNull()
        {
            // Arrange
            OrderDto? orderDto = null;

            // Act
            var result = OrderMapper.ToDomainModel(orderDto);

            // Assert
            Assert.Null(result);
        }
    }
}
