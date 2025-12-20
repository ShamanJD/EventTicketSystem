using EventTicket.Contracts;
using MassTransit;

namespace EventTicket.Notification.Consumers;

public class ConcertCreatedConsumer(ILogger<ConcertCreatedConsumer> logger) : IConsumer<ConcertCreatedEvent>
{
    public Task Consume(ConsumeContext<ConcertCreatedEvent> context)
    {
        logger.LogInformation("Woohoo! New concert created: {Name} at {Date}",
            context.Message.Name,
            context.Message.Date);

        return Task.CompletedTask;
    }
}