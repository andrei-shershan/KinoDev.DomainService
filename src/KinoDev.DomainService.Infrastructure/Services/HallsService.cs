using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.Shared.DtoModels.Hall;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IHallsService
    {
        Task<HallDto> CreateHallAsync(HallDto hallDto);
        Task<IEnumerable<HallDto>> GetAllHallsAsync();
        Task<HallDto> GetHallByIdAsync(int hallId);
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

        public async Task<HallDto> CreateHallAsync(HallDto hallDto)
        {
            if (hallDto == null)
            {
                throw new ArgumentNullException(nameof(hallDto));
            }

            var hall = hallDto.ToDomainModel();

            await _context.Halls.AddAsync(hall);
            await _context.SaveChangesAsync();

            return hall.ToDto();
        }

        public async Task<IEnumerable<HallDto>> GetAllHallsAsync()
        {
            var halls = await _context.Halls.ToListAsync();
            return halls.Select(h => h.ToDto());
        }

        public async Task<HallDto> GetHallByIdAsync(int hallId)
        {
            var hall = await _context.Halls
                .FirstOrDefaultAsync(h => h.Id == hallId);

            return hall?.ToDto();
        }
    }
}