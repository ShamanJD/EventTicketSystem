using EventTicket.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController(IPublishEndpoint publishEndpoint, ILogger<BookingController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var bookingId = Guid.NewGuid();
        
        // In a real app, we would save to DB here first with status "New"
        
        logger.LogInformation("Creating booking {BookingId} for amount {Amount}", bookingId, dto.Amount);

        await publishEndpoint.Publish(new BookingCreated(bookingId, dto.ConcertId, dto.Amount));

        return Accepted(new { BookingId = bookingId, Status = "Processing" });
    }
}

public record CreateBookingDto(Guid ConcertId, decimal Amount);
