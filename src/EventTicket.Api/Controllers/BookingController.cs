using EventTicket.Application.Services;
using EventTicket.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController(IPublishEndpoint publishEndpoint, ILogger<BookingController> logger, IApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var bookingId = Guid.NewGuid();
        
        logger.LogInformation("Creating booking {BookingId} for amount {Amount}", bookingId, dto.Amount);

        await publishEndpoint.Publish(new BookingCreated(bookingId, dto.ConcertId, dto.Amount));

        await context.SaveChangesAsync(ct);

        return Accepted(new { BookingId = bookingId, Status = "Processing" });
    }
}

public record CreateBookingDto(Guid ConcertId, decimal Amount);
