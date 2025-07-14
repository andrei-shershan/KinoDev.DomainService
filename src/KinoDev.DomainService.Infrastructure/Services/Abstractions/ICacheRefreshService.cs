namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
{
    public interface ICacheRefreshService
    {
        Task RefreshMoviesAsync(CancellationToken cancellationToken = default);
        Task RefreshShowTimesAsync(CancellationToken cancellationToken = default);
        Task RefreshOrdersAsync(CancellationToken cancellationToken = default);
        Task RefreshTicketsAsync(CancellationToken cancellationToken = default);
        Task RefreshHallsAsync(CancellationToken cancellationToken = default);
        Task RefreshSeatsAsync(CancellationToken cancellationToken = default);
        Task RefreshAllCachesAsync(CancellationToken cancellationToken = default);
    }
}