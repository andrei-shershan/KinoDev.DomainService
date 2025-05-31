using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.Shared.DtoModels.Hall;

namespace KinoDev.DomainService.Infrastructure.Mappers
{
    public static class HallMapper
    {
        public static HallDto ToDto(this Hall hall)
        {
            if (hall == null)
            {
                return null;
            }
            
            return new HallDto()
            {
                Id = hall.Id,
                Name = hall.Name,
            };
        }

        public static Hall ToDomainModel(this HallDto hallDto)
        {
            if (hallDto == null)
            {
                return null;
            }
            
            return new Hall()
            {
                Id = hallDto.Id,
                Name = hallDto.Name,
            };
        }
    }
}