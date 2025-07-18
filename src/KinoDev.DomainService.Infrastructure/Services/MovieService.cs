﻿using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.ShowingMovies;
using Microsoft.EntityFrameworkCore;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class MovieService : IMovieService
    {
        private readonly KinoDevDbContext _dbContext;

        private readonly ICacheRefreshService _cacheRefreshService;

        public MovieService(KinoDevDbContext dbContext, 
            ICacheRefreshService cacheRefreshService)
        {
            _dbContext = dbContext;
            _cacheRefreshService = cacheRefreshService;
        }   

        public async Task<MovieDto> CreateAsync(MovieDto movieDto)
        {
            var movie = movieDto.ToDomainModel();

            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.SaveChangesAsync();

            await _cacheRefreshService.RefreshMoviesAsync();

            return movie.ToDto();
        }

        public async Task<IEnumerable<MovieDto>> GetAllAsync()
        {
            // TODO: Add Pagination
            var movies = await _dbContext.Movies.ToListAsync();
            return movies.Select(x => x.ToDto());
        }

        public async Task<MovieDto> GetByIdAsync(int id)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            return movie?.ToDto();
        }

        public async Task<IEnumerable<ShowingMovie>> GetShowingMoviesAsync(DateTime date)
        {
            var dbResults = await
                _dbContext
                    .Movies
                    .Join(_dbContext.ShowTimes, m => m.Id, st => st.MovieId, (m, st) => new { m, st })
                    .Join(_dbContext.Halls, x => x.st.HallId, h => h.Id, (x, h) => new { x.m, x.st, h })
                    .Where(x => DateOnly.FromDateTime(x.st.Time.Date) == DateOnly.FromDateTime(date))
                    .ToListAsync();

            var now = DateTime.UtcNow;

            var result = new List<ShowingMovie>();
            foreach (var group in dbResults.GroupBy(x => x.m.Id))
            {
                var item = group.FirstOrDefault();
                if (item != null)
                {
                    var moviesShowTimeDetails = group.Select(st => new MovieShowTimeDetails
                    {
                        Id = st.st.Id,
                        HallId = item.h.Id,
                        HallName = item.h.Name,
                        Time = st.st.Time,
                        Price = st.st.Price,
                        IsSellingAvailable = st.st.Time > now
                    }).OrderBy(st => st.Time);

                    result.Add(new ShowingMovie
                    {
                        Id = item.m.Id,
                        Name = item.m.Name,
                        Description = item.m.Description,
                        ReleaseDate = item.m.ReleaseDate,
                        Duration = item.m.Duration,
                        Url = item.m.Url,
                        MoviesShowTimeDetails = moviesShowTimeDetails
                    });
                }
            }

            return result;
        }
    }
}
