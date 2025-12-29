using EventTicket.Application.Services;
using EventTicket.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventTicket.Infrastructure
{
    public class AuthDbContext : DbContext, IAuthDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
