using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services;

public class InitializerService : IHostedService
{
    private readonly KinoDevDbContext _dbContext;

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger<InitializerService> _logger;

    private readonly ICacheRefreshService _cacheRefreshService;

    public InitializerService(
        KinoDevDbContext dbContext,
        IDistributedCache distributedCache,
        ILogger<InitializerService> logger,
        ICacheRefreshService cacheRefreshService
        )
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheRefreshService = cacheRefreshService ?? throw new ArgumentNullException(nameof(cacheRefreshService));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"InitializerService is starting at {DateTime.UtcNow}");

        // TODO: Review this code, DRY and refactor if necessary
        var movies = await _distributedCache.GetStringAsync("Movies", cancellationToken);
        if (!string.IsNullOrWhiteSpace(movies))
        {
            _logger.LogInformation("Seeding movies from distributed cache...");
            var movieList = System.Text.Json.JsonSerializer.Deserialize<List<Movie>>(movies);
            if (movieList != null && movieList.Any())
            {
                // Ensure the database is not empty before seeding
                if (!await _dbContext.Movies.AnyAsync(cancellationToken))
                {
                    await _dbContext.Movies.AddRangeAsync(movieList, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            _logger.LogWarning("No movies found in distributed cache. Skipping seeding.");
        }

        var halls = await _distributedCache.GetStringAsync("Halls", cancellationToken);
        if (!string.IsNullOrWhiteSpace(halls))
        {
            _logger.LogInformation("Seeding halls from distributed cache...");
            var hallList = System.Text.Json.JsonSerializer.Deserialize<List<Hall>>(halls);
            if (hallList != null && hallList.Any())
            {
                // Ensure the database is not empty before seeding
                if (!await _dbContext.Halls.AnyAsync(cancellationToken))
                {
                    await _dbContext.Halls.AddRangeAsync(hallList, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            _logger.LogWarning("No halls found in distributed cache. Skipping seeding.");
        }

        var seats = await _distributedCache.GetStringAsync("Seats", cancellationToken);
        if (!string.IsNullOrWhiteSpace(seats))
        {
            _logger.LogInformation("Seeding seats from distributed cache...");
            var seatList = System.Text.Json.JsonSerializer.Deserialize<List<Seat>>(seats);
            if (seatList != null && seatList.Any())
            {
                // Ensure the database is not empty before seeding
                if (!await _dbContext.Seats.AnyAsync(cancellationToken))
                {
                    await _dbContext.Seats.AddRangeAsync(seatList, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            _logger.LogWarning("No seats found in distributed cache. Skipping seeding.");
        }

        var showTimes = await _distributedCache.GetStringAsync("ShowTimes", cancellationToken);
        if (!string.IsNullOrWhiteSpace(showTimes))
        {
            _logger.LogInformation("Seeding show times from distributed cache...");
            var showTimeList = System.Text.Json.JsonSerializer.Deserialize<List<ShowTime>>(showTimes);
            if (showTimeList != null && showTimeList.Any())
            {
                // Ensure the database is not empty before seeding
                if (!await _dbContext.ShowTimes.AnyAsync(cancellationToken))
                {
                    await _dbContext.ShowTimes.AddRangeAsync(showTimeList, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            _logger.LogWarning("No show times found in distributed cache. Skipping seeding.");
        }

        var orders = await _distributedCache.GetStringAsync("Orders", cancellationToken);
        if (!string.IsNullOrWhiteSpace(orders))
        {
            _logger.LogInformation("Seeding orders from distributed cache...");
            var orderList = System.Text.Json.JsonSerializer.Deserialize<List<Order>>(orders);
            if (orderList != null && orderList.Any())
            {
                // Ensure the database is not empty before seeding
                if (!await _dbContext.Orders.AnyAsync(cancellationToken))
                {
                    await _dbContext.Orders.AddRangeAsync(orderList, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            _logger.LogWarning("No orders found in distributed cache. Skipping seeding.");
        }

        var tickets = await _distributedCache.GetStringAsync("Tickets", cancellationToken);
        if (!string.IsNullOrWhiteSpace(tickets))
        {
            _logger.LogInformation("Seeding tickets from distributed cache...");
            var ticketList = System.Text.Json.JsonSerializer.Deserialize<List<Ticket>>(tickets);
            if (ticketList != null && ticketList.Any())
            {
                // Ensure the database is not empty before seeding
                if (!await _dbContext.Tickets.AnyAsync(cancellationToken))
                {
                    await _dbContext.Tickets.AddRangeAsync(ticketList, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        else
        {
            _logger.LogWarning("No tickets found in distributed cache. Skipping seeding.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}