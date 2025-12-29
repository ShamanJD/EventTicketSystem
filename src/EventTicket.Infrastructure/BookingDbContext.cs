using EventTicket.Application.Services;
using EventTicket.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventTicket.Infrastructure
{
    public class BookingDbContext : DbContext, IBookingDbContext
    {
        public DbSet<Booking> Bookings { get; set; }

        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
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
