using System.Text.Json;
using EventTicket.Application.DTOs;
using Microsoft.Extensions.Caching.Distributed;

namespace EventTicket.Application.Services;

public class CachedConcertService(IConcertService impl, IDistributedCache cache) : IConcertService
{
    public async Task<ConcertDto?> GetConcertByIdAsync(Guid id, CancellationToken ct)
    {
        var key = $"concert-{id}";

        var cachedMember = await cache.GetStringAsync(key, ct);

        if (!string.IsNullOrEmpty(cachedMember))
        {
            return JsonSerializer.Deserialize<ConcertDto>(cachedMember);
        }

        var concert = await impl.GetConcertByIdAsync(id, ct);

        if (concert is null) return null;

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(concert),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            ct);

        return concert;
    }

    public Task<ConcertDto> CreateConcertAsync(CreateConcertDto dto, CancellationToken ct)
    {
        return impl.CreateConcertAsync(dto, ct);
    }
}