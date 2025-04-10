using KinoDev.DomainService.Domain.Context;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.EntityFrameworkCore;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IShowTimeService
    {
        Task<ShowTimeDetailsDto> GetDetailsByIdAsync(int id);

        Task<ShowTimeSeatsDto> GetShowTimeSeatsAsync(int id);
    }

    public class ShowTimeService : IShowTimeService
    {
        private readonly KinoDevDbContext _dbContext;

        public ShowTimeService(KinoDevDbContext dbContext)
        {
            _dbContext = dbContext;
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

            return null;
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

            return null;
        }
    }
}