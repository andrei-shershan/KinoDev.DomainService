using KinoDev.DomainService.Domain.Context;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IShowTimeService
    {
        Task<IEnumerable<ShowTimeDetailsDto>> GetAllAsync(DateTime start, DateTime end);

        Task<ShowTimeDetailsDto> GetDetailsByIdAsync(int id);

        Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int id);
    }

    public class ShowTimeService : IShowTimeService
    {
        private readonly KinoDevDbContext _dbContext;

        private ILogger<ShowTimeService> _logger;

        public ShowTimeService(KinoDevDbContext dbContext, ILogger<ShowTimeService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
                .Select(x => new ShowTimeDetailsDto()
                {
                    Id = x.Id,
                    Time = x.Time,
                    Price = x.Price,
                    Movie = new MovieDto()
                    {
                        Id = x.Movie.Id,
                        Name = x.Movie.Name,
                        Description = x.Movie.Description,
                        ReleaseDate = x.Movie.ReleaseDate,
                        Duration = x.Movie.Duration,
                        Url = x.Movie.Url
                    },
                    Hall = new HallDto()
                    {
                        Id = x.Hall.Id,
                        Name = x.Hall.Name
                    }
                })
                .ToListAsync();

            if (dbShowTimes == null || !dbShowTimes.Any())
            {
                _logger.LogWarning($"No show times found between {startDate} and {endDate}.");
                return null;
            }

            return dbShowTimes
                .Select(x => new ShowTimeDetailsDto()
                {
                    Id = x.Id,
                    Time = x.Time,
                    Price = x.Price,
                    Movie = new MovieDto()
                    {
                        Id = x.Movie.Id,
                        Name = x.Movie.Name,
                        Description = x.Movie.Description,
                        ReleaseDate = x.Movie.ReleaseDate,
                        Duration = x.Movie.Duration,
                        Url = x.Movie.Url
                    },
                    Hall = new HallDto()
                    {
                        Id = x.Hall.Id,
                        Name = x.Hall.Name
                    }
                });
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
                return new ShowTimeDetailsDto()
                {
                    Id = dbResult.Id,
                    Time = dbResult.Time,
                    Price = dbResult.Price,
                    Movie = new MovieDto()
                    {
                        Id = dbResult.Movie.Id,
                        Name = dbResult.Movie.Name,
                        Description = dbResult.Movie.Description,
                        ReleaseDate = dbResult.Movie.ReleaseDate,
                        Duration = dbResult.Movie.Duration,
                        Url = dbResult.Movie.Url
                    },
                    Hall = new HallDto()
                    {
                        Id = dbResult.Hall.Id,
                        Name = dbResult.Hall.Name
                    }
                };
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