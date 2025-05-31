using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.Shared.DtoModels.ShowTimes;

namespace KinoDev.DomainService.Infrastructure.Mappers
{
    public static class ShowTimeDetailsMapper
    {
        public static ShowTimeDetailsDto ToDto(this ShowTime showTime)
        {
            if (showTime == null)
            {
                return null;
            }

            return new ShowTimeDetailsDto()
            {
                Id = showTime.Id,
                Time = showTime.Time,
                Price = showTime.Price,
                Movie = showTime.Movie.ToDto(),
                Hall = showTime.Hall.ToDto(),
            };
        }
    }
}