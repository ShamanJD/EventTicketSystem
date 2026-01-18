using EventTicket.Application.DTOs;
using EventTicket.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventTicket.Application.Features.Concerts.Queries.GetAllConcerts;

public class GetAllConcertsQueryHandler(IConcertsDbContext context) 
    : IRequestHandler<GetAllConcertsQuery, List<ConcertDto>>
{
    public async Task<List<ConcertDto>> Handle(GetAllConcertsQuery request, CancellationToken cancellationToken)
    {
        var concerts = await context.Concerts
            .AsNoTracking()
            .Select(c => new ConcertDto(c.Id, c.Name, c.Date, c.Venue))
            .ToListAsync(cancellationToken);

        return concerts;
    }
}
