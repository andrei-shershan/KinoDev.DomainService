using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.ShowTimes;

namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
{
    public interface IShowTimesService
    {
        Task<IEnumerable<ShowTimeDetailsDto>> GetAllAsync(DateTime start, DateTime end);

        Task<ShowTimeDetailsDto> GetDetailsByIdAsync(int id);

        Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int id);

        Task<bool> CreateAsync(CreateShowTimeRequest request);
    }
}