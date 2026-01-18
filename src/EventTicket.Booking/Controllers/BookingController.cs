using EventTicket.Application.Features.Bookings.Commands.CreateBooking;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Booking.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateBookingCommand(dto.ConcertId, dto.Amount), ct);

        if (!result.Success)
            return NotFound(new { Error = result.Error, ConcertId = dto.ConcertId });

        return Accepted(new { BookingId = result.BookingId, Status = "Processing" });
    }
}

public record CreateBookingRequest(Guid ConcertId, decimal Amount);
