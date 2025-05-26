using System.Transactions;
using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Seats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IHallsService
    {
        Task<HallSummary> CreateHallAsync(string hallName, int rowsCount, int seatsCount);
        Task<IEnumerable<HallSummary>> GetAllHallsAsync();
        Task<HallSummary> GetHallByIdAsync(int hallId);
    }

    public class HallsService : IHallsService
    {
        private readonly KinoDevDbContext _context;

        private readonly ILogger<HallsService> _logger;

        public HallsService(KinoDevDbContext context, ILogger<HallsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HallSummary> CreateHallAsync(string hallName, int rowsCount, int seatsCount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var hall = new Hall()
            {
                Name = hallName
            };

            try
            {
                await _context.Halls.AddAsync(hall);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating hall.");
                await transaction.RollbackAsync();
                return null;
            }

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

            try
            {
                await _context.Seats.AddRangeAsync(seats);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding seats to hall.");
                await transaction.RollbackAsync();
                return null;
            }

            await transaction.CommitAsync();

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
                }).ToList(),
            };
        }

        public async Task<IEnumerable<HallSummary>> GetAllHallsAsync()
        {
            var hallsWithSeats = await _context.Halls.Include(h => h.Seats)
                .Include(h => h.ShowTimes)
                .ToListAsync();

            if (hallsWithSeats == null || !hallsWithSeats.Any())
            {
                _logger.LogWarning("No halls found in the database.");
                return null;
            }

            return hallsWithSeats.Select(h => new HallSummary
            {
                Id = h.Id,
                Name = h.Name,
                Seats = h.Seats.Select(s => new SeatDto
                {
                    Id = s.Id,
                    Row = s.Row,
                    Number = s.Number,
                    HallId = s.HallId,
                }).ToList(),
            });
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

            return new HallSummary
            {
                Id = hallWithSeats.Id,
                Name = hallWithSeats.Name,
                Seats = hallWithSeats.Seats.Select(s => new SeatDto
                {
                    Id = s.Id,
                    Row = s.Row,
                    Number = s.Number,
                    HallId = s.HallId,
                }).ToList(),
            };
        }
    }
}