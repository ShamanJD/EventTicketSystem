using EventTicket.Contracts;
using MassTransit;

namespace EventTicket.Payment.Consumers;

public class PaymentRequestConsumer(ILogger<PaymentRequestConsumer> logger) : IConsumer<RequestPayment>
{
    public async Task Consume(ConsumeContext<RequestPayment> context)
    {
        logger.LogInformation("Processing payment for Booking {BookingId}, Amount: {Amount}", context.Message.BookingId, context.Message.Amount);

        // Simple logic for demo: If amount is > 1000, fail it.
        if (context.Message.Amount > 1000)
        {
            logger.LogError("Payment declined for Booking {BookingId}. Amount too high.", context.Message.BookingId);
            await context.Publish(new PaymentFailed(context.Message.BookingId, "Amount exceeds limit"));
        }
        else
        {
            logger.LogInformation("Payment successful for Booking {BookingId}", context.Message.BookingId);
            await context.Publish(new PaymentCompleted(context.Message.BookingId));
        }
    }
}
