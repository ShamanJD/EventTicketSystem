using EventTicket.Application.Services;
using EventTicket.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventTicket.Infrastructure
{
    public class ConcertsDbContext : DbContext, IConcertsDbContext
    {
        public DbSet<Concert> Concerts { get; set; }

        public ConcertsDbContext(DbContextOptions<ConcertsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
