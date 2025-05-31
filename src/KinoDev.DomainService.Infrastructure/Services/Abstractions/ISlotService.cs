using KinoDev.Shared.DtoModels.ShowTimes;

namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
{
    public interface ISlotService
    {
        Task<ShowTimeForDateDto> GetShowTimeSlotsAsync(DateTime date);
    }
}