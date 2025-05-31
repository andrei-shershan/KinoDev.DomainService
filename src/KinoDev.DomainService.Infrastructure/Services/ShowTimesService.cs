using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowTimes;
using KinoDev.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class ShowTimesService : IShowTimesService
    {
        private readonly KinoDevDbContext _dbContext;

        private ILogger<ShowTimesService> _logger;

        public ShowTimesService(KinoDevDbContext dbContext, ILogger<ShowTimesService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateShowTimeRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var movie = await _dbContext.Movies.FindAsync(request.MovieId);
                if (movie == null)
                {
                    _logger.LogError($"Movie with id {request.MovieId} not found.");
                    return false;
                }

                var hall = await _dbContext.Halls.FindAsync(request.HallId);
                if (hall == null)
                {
                    _logger.LogError($"Hall with id {request.HallId} not found.");
                    return false;
                }

                var showTime = new ShowTime()
                {
                    MovieId = request.MovieId,
                    HallId = request.HallId,
                    Time = request.Time,
                    Price = request.Price
                };

                await _dbContext.ShowTimes.AddAsync(showTime);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating show time.");
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<ShowTimeDetailsDto>> GetAllAsync(DateTime startDate, DateTime endDate)
        {
            // Adjust date time to use date only
            startDate = startDate.Date;
            endDate = endDate.Date;

            var dbShowTimes = await _dbContext
                .ShowTimes
                .Include(x => x.Hall)
                .Include(x => x.Movie)
                .Where(x => x.Time.Date >= startDate && x.Time.Date <= endDate)
                .ToListAsync();

            if (dbShowTimes.IsNullOrEmptyCollection())
            {
                _logger.LogWarning($"No show times found between {startDate} and {endDate}.");
                return null;
            }

            return dbShowTimes
                .Select(x => x.ToDto());
        }

        public async Task<ShowTimeDetailsDto> GetDetailsByIdAsync(int id)
        {
            var dbResult = await _dbContext
                .ShowTimes
                    .Include(x => x.Hall)
                    .Include(x => x.Movie)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (dbResult != null)
            {
                return dbResult.ToDto();
            }
            else
            {
                _logger.LogError($"ShowTime with id {id} not found.");
                return null;
            }
        }

        public async Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int id)
        {
            var dbShowTimesWithSeats = await _dbContext.ShowTimes
                .Join(_dbContext.Halls, st => st.HallId, h => h.Id, (st, h) => new { ShowTime = st, Hall = h })
                .Join(_dbContext.Seats, sth => sth.Hall.Id, s => s.HallId, (sth, s) => new { ShowTime = sth.ShowTime, Hall = sth.Hall, Seat = s })
                .Where(x => x.ShowTime.Id == id)
                .ToListAsync();

            var dbShowTimeTickets = await _dbContext.Tickets
                .Where(x => x.ShowTimeId == id)
                .ToListAsync();

            var dbShowWithTicket = dbShowTimesWithSeats?.FirstOrDefault();
            if (dbShowWithTicket != null)
            {
                return new ShowTimeSeatsDto()
                {
                    Id = dbShowWithTicket.ShowTime.Id,
                    HallId = dbShowWithTicket.Hall.Id,
                    Time = dbShowWithTicket.ShowTime.Time,
                    Price = dbShowWithTicket.ShowTime.Price,
                    Seats = dbShowWithTicket.Hall.Seats.Select(x => new ShowTimeSeatDto()
                    {
                        Id = x.Id,
                        Row = x.Row,
                        Number = x.Number,
                        IsAvailable = dbShowTimeTickets?.FirstOrDefault(stt => stt.SeatId == x.Id) == null
                    })
                };
            }
            else
            {
                _logger.LogError($"ShowTime with id {id} not found.");
                return null;
            }
        }
    }
}