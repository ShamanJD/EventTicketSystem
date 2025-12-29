using EventTicket.Application.Services;
using EventTicket.Contracts;
using EventTicket.Domain;
using MassTransit;

namespace EventTicket.Booking.Consumers;

public class BookingCreatedConsumer(ILogger<BookingCreatedConsumer> logger, IServiceProvider serviceProvider) : 
    IConsumer<BookingCreated>
{
    public async Task Consume(ConsumeContext<BookingCreated> context)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();

        var booking = new Domain.Booking(context.Message.BookingId, context.Message.ConcertId, context.Message.Amount);
        
        await dbContext.Bookings.AddAsync(booking, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Booking created in database: {BookingId}", context.Message.BookingId);
    }
}
