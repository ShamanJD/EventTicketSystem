using EventTicket.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventTicket.Application.Services;

public interface IApplicationDbContext
{
    DbSet<Concert> Concerts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}