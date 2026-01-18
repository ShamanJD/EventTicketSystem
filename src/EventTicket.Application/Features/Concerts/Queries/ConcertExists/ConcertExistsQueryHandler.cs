using EventTicket.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventTicket.Application.Features.Concerts.Queries.ConcertExists;

public class ConcertExistsQueryHandler(IConcertsDbContext context) 
    : IRequestHandler<ConcertExistsQuery, bool>
{
    public async Task<bool> Handle(ConcertExistsQuery request, CancellationToken cancellationToken)
    {
        return await context.Concerts
            .AnyAsync(c => c.Id == request.ConcertId, cancellationToken);
    }
}
