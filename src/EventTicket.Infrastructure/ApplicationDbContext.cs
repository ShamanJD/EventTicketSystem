using EventTicket.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EventTicket.Application.Services;
using MassTransit;

namespace EventTicket.Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Concert> Concerts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            modelBuilder.Entity<Booking>()
                .Property(x => x.RowVersion)
                .IsRowVersion();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
