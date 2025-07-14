using KinoDev.DomainService.Infrastructure.Services.Abstractions;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class CacheRefreshEmptyService : ICacheRefreshService
    {
        public Task RefreshAllCachesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RefreshHallsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RefreshMoviesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RefreshOrdersAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RefreshSeatsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RefreshShowTimesAsync(CancellationToken cancellationToken = default)
        {
           return Task.CompletedTask;
        }

        public Task RefreshTicketsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}