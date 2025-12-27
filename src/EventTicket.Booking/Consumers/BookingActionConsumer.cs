using EventTicket.Contracts;
using MassTransit;

namespace EventTicket.Booking.Consumers;

public class BookingActionConsumer(ILogger<BookingActionConsumer> logger) : 
    IConsumer<ConfirmBooking>,
    IConsumer<CancelBooking>
{
    public Task Consume(ConsumeContext<ConfirmBooking> context)
    {
        logger.LogInformation("Booking Confirmed: {BookingId}. Ticket issued.", context.Message.BookingId);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<CancelBooking> context)
    {
        logger.LogWarning("Booking Cancelled: {BookingId}. Reason: {Reason}", context.Message.BookingId, context.Message.Reason);
        return Task.CompletedTask;
    }
}
