using MediatR;

namespace EventTicket.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(Guid ConcertId, decimal Amount) : IRequest<CreateBookingResult>;

public record CreateBookingResult(bool Success, Guid? BookingId = null, string? Error = null);
