using EventTicket.Application.Services;
using EventTicket.Contracts;
using MassTransit;

namespace EventTicket.Booking.Consumers;

public class BookingActionConsumer(ILogger<BookingActionConsumer> logger, IServiceProvider serviceProvider) : 
    IConsumer<ConfirmBooking>,
    IConsumer<CancelBooking>
{
    public async Task Consume(ConsumeContext<ConfirmBooking> context)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();

        var booking = await dbContext.Bookings.FindAsync(context.Message.BookingId);
        if (booking == null)
        {
            logger.LogError("Booking not found: {BookingId}", context.Message.BookingId);
            return;
        }

        booking.Confirm();
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Booking Confirmed: {BookingId}. Ticket issued.", context.Message.BookingId);
    }

    public async Task Consume(ConsumeContext<CancelBooking> context)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();

        var booking = await dbContext.Bookings.FindAsync(context.Message.BookingId);
        if (booking == null)
        {
             logger.LogError("Booking not found: {BookingId}", context.Message.BookingId);
             return;
        }

        booking.Cancel(context.Message.Reason);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogWarning("Booking Cancelled: {BookingId}. Reason: {Reason}", context.Message.BookingId, context.Message.Reason);
    }
}
