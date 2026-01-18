using System.Text.Json;
using EventTicket.Application.DTOs;
using EventTicket.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EventTicket.Application.Features.Concerts.Queries.GetConcertById;

public class GetConcertByIdQueryHandler(
    IConcertsDbContext context,
    IDistributedCache cache) : IRequestHandler<GetConcertByIdQuery, ConcertDto?>
{
    public async Task<ConcertDto?> Handle(GetConcertByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"concert-{request.Id}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<ConcertDto>(cachedData);
        }

        var concert = await context.Concerts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (concert is null)
        {
            return null;
        }

        var dto = new ConcertDto(concert.Id, concert.Name, concert.Date, concert.Venue);

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(dto),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return dto;
    }
}
