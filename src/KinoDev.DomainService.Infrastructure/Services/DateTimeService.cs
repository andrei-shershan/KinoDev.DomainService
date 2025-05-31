using KinoDev.DomainService.Infrastructure.Services.Abstractions;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
