using MassTransit;
using EventTicket.Contracts;

namespace EventTicket.Orchestrator.Consumers;

public class DebugPaymentConsumer : IConsumer<PaymentCompleted>
{
    private readonly ILogger<DebugPaymentConsumer> _logger;

    public DebugPaymentConsumer(ILogger<DebugPaymentConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<PaymentCompleted> context)
    {
        _logger.LogWarning("DEBUG: Received PaymentCompleted for {BookingId}", context.Message.BookingId);
        return Task.CompletedTask;
    }
}
