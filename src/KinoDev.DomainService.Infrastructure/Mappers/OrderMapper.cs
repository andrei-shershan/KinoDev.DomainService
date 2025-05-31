using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.Shared.DtoModels.Orders;

namespace KinoDev.DomainService.Infrastructure.Mappers
{
    public static class OrderMapper
    {
        /// <summary>
        /// Maps an Order domain model to an OrderDto
        /// </summary>
        /// <param name="order">The Order domain model to map from</param>
        /// <returns>The mapped OrderDto object, or null if input is null</returns>
        public static OrderDto ToDto(this Order order)
        {
            if (order == null)
            {
                return null;
            }

            return new OrderDto()
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                Cost = order.Cost,
                State = order.State,
                Email = order.Email,
                EmailSent = order.EmailSent,
                UserId = order.UserId,
                FileUrl = order.FileUrl
            };
        }

        /// <summary>
        /// Maps an OrderDto to an Order domain model
        /// </summary>
        /// <param name="orderDto">The OrderDto to map from</param>
        /// <returns>The mapped Order domain model, or null if input is null</returns>
        public static Order ToDomainModel(this OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return null;
            }

            return new Order()
            {
                Id = orderDto.Id,
                CreatedAt = orderDto.CreatedAt,
                CompletedAt = orderDto.CompletedAt,
                Cost = orderDto.Cost,
                State = orderDto.State,
                Email = orderDto.Email,
                EmailSent = orderDto.EmailSent,
                UserId = orderDto.UserId,
                FileUrl = orderDto.FileUrl
            };
        }
    }
}
