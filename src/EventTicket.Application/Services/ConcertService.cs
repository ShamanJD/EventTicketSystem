using EventTicket.Application.DTOs;
using EventTicket.Contracts;
using EventTicket.Domain.Exceptions;
using EventTicket.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventTicket.Application.Services;

public class ConcertService(IConcertsDbContext context, IPublishEndpoint publishEndpoint, ILogger<ConcertService> logger) : IConcertService
{
    public async Task<ConcertDto> CreateConcertAsync(CreateConcertDto dto, CancellationToken cancellationToken)
    {
        var concert = new Concert(dto.Name, dto.Date, dto.Venue);

        if (dto.Date < DateTime.Today) throw new InvalidConcertDataException("Date must be in future");

        await context.Concerts.AddAsync(concert, cancellationToken);

        await publishEndpoint.Publish(new ConcertCreatedEvent(concert.Id, concert.Name, concert.Date), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Concert {ConcertName} created successfully with Id {ConcertId}", concert.Name, concert.Id);

        return MapToDto(concert);
    }

    public async Task<ConcertDto?> GetConcertByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var concert = await context.Concerts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (concert is null)
        {
            return null;
        }

        return MapToDto(concert);
    }

    public Task<List<ConcertDto>> GetAllConcertsAsync()
    {
        var concerts = context.Concerts;
        var concertDtos = concerts.Select(x => MapToDto(x)).ToListAsync();
        return concertDtos;
    }

    private static ConcertDto MapToDto(Concert concert)
    {
        return new ConcertDto(concert.Id, concert.Name, concert.Date, concert.Venue);
    }
}