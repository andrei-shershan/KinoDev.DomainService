using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class CacheRefreshService : ICacheRefreshService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CacheRefreshService> _logger;
        private readonly KinoDevDbContext _dbContext;
        private const int DaysToExpire = 30;

        public CacheRefreshService(
            KinoDevDbContext dbContext,
            IDistributedCache distributedCache,
             ILogger<CacheRefreshService> logger
             )
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task RefreshAllCachesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Refreshing all caches...");
            await RefreshMoviesAsync(cancellationToken);
            await RefreshOrdersAsync(cancellationToken);
            await RefreshShowTimesAsync(cancellationToken);
            await RefreshTicketsAsync(cancellationToken);
            await RefreshHallsAsync(cancellationToken);
            await RefreshSeatsAsync(cancellationToken);
            _logger.LogInformation("All caches refreshed successfully.");
        }

        public async Task RefreshMoviesAsync(CancellationToken cancellationToken = default)
        {
            var movies = await _dbContext.Movies.ToListAsync(cancellationToken);
            _logger.LogInformation($"Found {movies?.Count ?? 0} movies in the database.");
            if (movies?.Any() ?? false)
            {
                var adjustedMovies = movies.Select(m => new
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    ReleaseDate = m.ReleaseDate,
                    Duration = m.Duration,
                    Url = m.Url
                }).ToList();

                await _distributedCache.SetStringAsync(
                    "Movies",
                    System.Text.Json.JsonSerializer.Serialize(adjustedMovies),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(DaysToExpire)
                    },
                    cancellationToken
                );
            }
        }

        public async Task RefreshOrdersAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _dbContext.Orders.ToListAsync(cancellationToken);
            _logger.LogInformation($"Found {orders?.Count ?? 0} orders in the database.");
            if (orders?.Any() ?? false)
            {
                var adjustedOrders = orders.Select(o => new
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    Email = o.Email,
                    Cost = o.Cost,
                    State = o.State,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    EmailSent = o.EmailSent,
                    FileUrl = o.FileUrl,
                }).ToList();

                await _distributedCache.SetStringAsync(
                    "Orders",
                    System.Text.Json.JsonSerializer.Serialize(adjustedOrders),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(DaysToExpire)
                    },
                    cancellationToken
                );
            }
        }

        public async Task RefreshShowTimesAsync(CancellationToken cancellationToken = default)
        {
            var showTimes = await _dbContext.ShowTimes.ToListAsync(cancellationToken);
            _logger.LogInformation($"Found {showTimes?.Count ?? 0} show times in the database.");
            if (showTimes?.Any() ?? false)
            {
                var adjustedShowTimes = showTimes.Select(st => new
                {
                    Id = st.Id,
                    MovieId = st.MovieId,
                    HallId = st.HallId,
                    Time = st.Time,
                    Price = st.Price
                }).ToList();

                await _distributedCache.SetStringAsync(
                    "ShowTimes",
                    System.Text.Json.JsonSerializer.Serialize(adjustedShowTimes),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(DaysToExpire)
                    },
                    cancellationToken
                );
            }
        }

        public async Task RefreshTicketsAsync(CancellationToken cancellationToken = default)
        {
            var tickets = await _dbContext.Tickets.ToListAsync(cancellationToken);
            _logger.LogInformation($"Found {tickets?.Count ?? 0} tickets in the database.");
            if (tickets?.Any() ?? false)
            {
                var adjustedTickets = tickets.Select(t => new
                {
                    Id = t.Id,
                    OrderId = t.OrderId,
                    ShowTimeId = t.ShowTimeId,
                    SeatId = t.SeatId,
                }).ToList();

                await _distributedCache.SetStringAsync(
                    "Tickets",
                    System.Text.Json.JsonSerializer.Serialize(adjustedTickets),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(DaysToExpire)
                    },
                    cancellationToken
                );
            }
        }

        public async Task RefreshHallsAsync(CancellationToken cancellationToken = default)
        {
            var halls = await _dbContext.Halls.ToListAsync(cancellationToken);
            _logger.LogInformation($"Found {halls?.Count ?? 0} halls in the database.");
            if (halls?.Any() ?? false)
            {
                var adjustedHalls = halls.Select(h => new
                {
                    Id = h.Id,
                    Name = h.Name,
                }).ToList();

                await _distributedCache.SetStringAsync(
                    "Halls",
                    System.Text.Json.JsonSerializer.Serialize(adjustedHalls),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(DaysToExpire)
                    },
                    cancellationToken
                );
            }

            await RefreshSeatsAsync(cancellationToken);
        }

        public async Task RefreshSeatsAsync(CancellationToken cancellationToken = default)
        {
            var seats = await _dbContext.Seats.ToListAsync(cancellationToken);
            _logger.LogInformation($"Found {seats?.Count ?? 0} seats in the database.");
            if (seats?.Any() ?? false)
            {
                var adjustedSeats = seats.Select(s => new
                {
                    Id = s.Id,
                    HallId = s.HallId,
                    Row = s.Row,
                    Number = s.Number
                }).ToList();

                await _distributedCache.SetStringAsync(
                    "Seats",
                    System.Text.Json.JsonSerializer.Serialize(adjustedSeats),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(DaysToExpire)
                    },
                    cancellationToken
                );
            }
        }
    }
}