﻿namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IDateTimeService
    {
        DateTime UtcNow();
    }

    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
