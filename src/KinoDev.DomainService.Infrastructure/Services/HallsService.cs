using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.ConfigurationModels;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Seats;
using KinoDev.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class HallsService : TransactionService, IHallsService
    {
        private readonly KinoDevDbContext _context;

        private readonly ILogger<HallsService> _logger;

        private readonly ICacheRefreshService _cacheRefreshService;

        public HallsService(
            KinoDevDbContext context,
            ILogger<HallsService> logger,
            IOptions<InMemoryDbSettings> inMemoryDbSettings,
            ICacheRefreshService cacheRefreshService) : base(inMemoryDbSettings)
        {
            _cacheRefreshService = cacheRefreshService ?? throw new ArgumentNullException(nameof(cacheRefreshService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<HallSummary> CreateHallAsync(string hallName, int rowsCount, int seatsCount)
        {
            using var transaction = await BeginTransactionAsync(_context);

            try
            {
                var hall = new Hall()
                {
                    Name = hallName
                };
                await _context.Halls.AddAsync(hall);
                await _context.SaveChangesAsync();

                var seats = new List<Seat>();
                for (int row = 1; row <= rowsCount; row++)
                {
                    for (int seatNumber = 1; seatNumber <= seatsCount; seatNumber++)
                    {
                        seats.Add(new Seat
                        {
                            Row = row,
                            Number = seatNumber,
                            HallId = hall.Id
                        });
                    }
                }

                await _context.Seats.AddRangeAsync(seats);
                await _context.SaveChangesAsync();

                await CommitTransactionAsync(transaction);

                await _cacheRefreshService.RefreshHallsAsync();

                return GetHallSummary(hall, seats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creatin hall with name {HallName}", hallName);
                await RollbackTransactionAsync(transaction);
                return null;
            }
        }

        public async Task<IEnumerable<HallSummary>> GetAllHallsAsync()
        {
            var hallsWithSeats = await _context.Halls.Include(h => h.Seats)
                .Include(h => h.ShowTimes)
                .ToListAsync();

            if (hallsWithSeats.IsNullOrEmptyCollection())
            {
                _logger.LogWarning("No halls found in the database.");
                return null;
            }

            return hallsWithSeats.Select(h => GetHallSummary(h, h.Seats));
        }

        public async Task<HallSummary> GetHallByIdAsync(int hallId)
        {
            var hallWithSeats = await _context
                .Halls
                .Include(h => h.Seats)
                .FirstOrDefaultAsync(h => h.Id == hallId);

            if (hallWithSeats == null)
            {
                _logger.LogWarning($"Hall with ID {hallId} not found.");
                return null;
            }

            return GetHallSummary(hallWithSeats, hallWithSeats.Seats);
        }

        private HallSummary GetHallSummary(Hall hall, IEnumerable<Seat> seats)
        {
            return new HallSummary
            {
                Id = hall.Id,
                Name = hall.Name,
                Seats = seats.Select(s => new SeatDto
                {
                    Id = s.Id,
                    Row = s.Row,
                    Number = s.Number,
                    HallId = s.HallId,
                })
            };
        }
    }
}