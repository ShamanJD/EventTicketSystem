using EventTicket.Contracts;
using MassTransit;

namespace EventTicket.Notification.Consumers;

public class SendNotificationConsumer(ILogger<SendNotificationConsumer> logger) : IConsumer<SendNotificationCommand>
{
    public Task Consume(ConsumeContext<SendNotificationCommand> context)
    {
        logger.LogInformation("Orchestration Works! Notification sent needed for concert: {Name} at {Date}",
            context.Message.Name,
            context.Message.Date);

        return Task.CompletedTask;
    }
}
