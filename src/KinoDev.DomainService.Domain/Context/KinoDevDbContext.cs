using KinoDev.DomainService.Domain.Constants;
using KinoDev.DomainService.Domain.DomainsModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KinoDev.DomainService.Domain.Context
{
    public class KinoDevDbContext : DbContext
    {
        public DbSet<Hall> Halls { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<ShowTime> ShowTimes { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public KinoDevDbContext(DbContextOptions<KinoDevDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreating(modelBuilder.Entity<Order>());
            OnModelCreating(modelBuilder.Entity<Ticket>());

            OverrideDefaultTypes(modelBuilder);

            OnModelCreating(modelBuilder.Entity<Hall>());
            OnModelCreating(modelBuilder.Entity<Movie>());

            base.OnModelCreating(modelBuilder);

            if (Database.IsInMemory())
            {
                var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                    v => v, // Don't change the input value
                    v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)); // Force Unspecified kind

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var dateTimeProperties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                    foreach (var property in dateTimeProperties)
                    {
                        modelBuilder.Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(dateTimeConverter);
                    }
                }
            }
        }

        private void OnModelCreating(EntityTypeBuilder<Hall> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(MySqlConstants.SHORT_NAME_MAX_LENGTH);
        }

        private void OnModelCreating(EntityTypeBuilder<Movie> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(MySqlConstants.SHORT_NAME_MAX_LENGTH);
            builder.Property(x => x.Description).HasMaxLength(MySqlConstants.DESCRIPTION_MAX_LENGTH);
        }

        private void OnModelCreating(EntityTypeBuilder<Order> builder)
        {
            OnGuidModelCreate(builder);

            builder
                .Property(t => t.UserId)
                .HasColumnType("char(36)")
                .IsRequired(false)
                .HasDefaultValue(null);

            builder
                .Property(t => t.Email)
                .IsRequired(false)
                .HasMaxLength(MySqlConstants.EMAIL_MAX_LENGTH)
                .HasDefaultValue(null);

            builder
                .Property(t => t.FileUrl)
                .IsRequired(false)
                .HasMaxLength(MySqlConstants.DESCRIPTION_MAX_LENGTH)
                .HasDefaultValue(null);
        }

        private void OnModelCreating(EntityTypeBuilder<Ticket> builder)
        {
            OnGuidModelCreate(builder);
        }

        private void OnGuidModelCreate<T>(EntityTypeBuilder<T> builder) where T : BaseGuidEntity
        {
            builder
                .Property(t => t.Id)
                .HasColumnType("char(36)")
                .HasDefaultValueSql("(UUID())");
        }

        private void OverrideDefaultTypes(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(Nullable<DateTime>))
                    {
                        property.SetColumnType(MySqlConstants.DATETIME_0_DEF);
                    }

                    if (property.ClrType == typeof(decimal))
                    {
                        property.SetColumnType(MySqlConstants.DECIMAL_182_DEF);
                    }
                }
            }
        }
    }
}
