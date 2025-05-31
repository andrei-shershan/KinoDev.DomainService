using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowTimes;
using Microsoft.EntityFrameworkCore;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class SlotService : ISlotService
    {
        private readonly KinoDevDbContext _dbContext;

        public SlotService(KinoDevDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShowTimeForDateDto> GetShowTimeSlotsAsync(DateTime date)
        {
            var query =
            from h in _dbContext.Halls
                // LEFT JOIN ShowTimes with date filter
            join st0 in _dbContext.ShowTimes
                        .Where(st => st.Time >= date.Date && st.Time < date.Date.AddDays(1))
            on h.Id equals st0.HallId into stGroup
            from st in stGroup.DefaultIfEmpty()
                // LEFT JOIN Movies
            join m0 in _dbContext.Movies
            on st.MovieId equals m0.Id into mGroup
            from m in mGroup.DefaultIfEmpty()
            select new
            {
                Hall = h,
                ShowTime = st,    // will be null if no matching ShowTime in window
                Movie = m     // will be null if no matching Movie
            };

            var dbResult = await query.ToListAsync();
            var hallWithMovies = dbResult
                .GroupBy(x => x.Hall)
                .Select(g => new HallWithMoviesDto
                {
                    Hall = new HallDto
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                    },
                    Movies = g
                        .Where(x => x.ShowTime != null && x.Movie != null)
                        .Select(x => new MovieWithShowTime
                        {
                            Id = x.Movie.Id,
                            Name = x.Movie.Name,
                            Duration = x.Movie.Duration,
                            Description = x.Movie.Description,
                            ReleaseDate = x.Movie.ReleaseDate,
                            Time = x.ShowTime.Time,
                        })
                })
                .ToList();

            return new ShowTimeForDateDto
            {
                Date = date,
                HallWithMovies = hallWithMovies
            };
        }
    }
}