using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Infrastructure.ConfigurationModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public abstract class TransactionService
    {
        private readonly bool _useTransactions;

        public TransactionService(IOptions<InMemoryDbSettings> inMemoryDbSettings)
        {
            // We use transactions only if InMemoryDb is not enabled.
            _useTransactions = !inMemoryDbSettings?.Value?.Enabled ?? false;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(KinoDevDbContext context)
        {
            if (_useTransactions)
            {
                return await context.Database.BeginTransactionAsync();
            }

            return null;
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }
        }
    }
}