using EventTicket.Contracts;
using MassTransit;

namespace EventTicket.Orchestrator.Sagas;

public class ConcertSaga : MassTransitStateMachine<ConcertSagaState>
{
    public State NotificationSent { get; private set; } = null!;

    public Event<ConcertCreatedEvent> ConcertCreated { get; private set; } = null!;

    public ConcertSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ConcertCreated, x => x.CorrelateById(m => m.Message.ConcertId));

        Initially(
            When(ConcertCreated)
                .Then(context =>
                {
                    context.Saga.Name = context.Message.Name;
                    context.Saga.Date = context.Message.Date;
                })
                .Publish(context =>
                    new SendNotificationCommand(context.Saga.CorrelationId, context.Saga.Name, context.Saga.Date))
                .TransitionTo(NotificationSent)
        );
    }
}
