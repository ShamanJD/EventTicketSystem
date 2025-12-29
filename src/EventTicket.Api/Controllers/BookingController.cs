using EventTicket.Application.Services;
using EventTicket.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventTicket.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController(
    IPublishEndpoint publishEndpoint,
    IConcertsDbContext concertsDbContext,
    ILogger<BookingController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var concertExists = await concertsDbContext.Concerts
            .AnyAsync(c => c.Id == dto.ConcertId, ct);

        if (!concertExists)
        {
            logger.LogWarning("Concert {ConcertId} not found", dto.ConcertId);
            return NotFound(new { Error = "Concert not found", ConcertId = dto.ConcertId });
        }

        var bookingId = Guid.NewGuid();

        logger.LogInformation("Creating booking {BookingId} for concert {ConcertId} with amount {Amount}", 
            bookingId, dto.ConcertId, dto.Amount);

        await publishEndpoint.Publish(new BookingCreated(bookingId, dto.ConcertId, dto.Amount), ct);

        return Accepted(new { BookingId = bookingId, Status = "Processing" });
    }
}

public record CreateBookingDto(Guid ConcertId, decimal Amount);
