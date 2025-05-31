using KinoDev.Shared.DtoModels.Hall;

namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
{
    public interface IHallsService
    {
        Task<HallSummary> CreateHallAsync(string hallName, int rowsCount, int seatsCount);
        Task<IEnumerable<HallSummary>> GetAllHallsAsync();
        Task<HallSummary> GetHallByIdAsync(int hallId);
    }
}