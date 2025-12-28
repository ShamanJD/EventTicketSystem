using EventTicket.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using EventTicket.Orchestrator.Hubs;

namespace EventTicket.Orchestrator.Sagas;

public class BookingSaga : MassTransitStateMachine<BookingSagaState>
{
    public State PaymentPending { get; private set; } = null!;
    public State Confirmed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;

    public Event<BookingCreated> BookingCreated { get; private set; } = null!;
    public Event<PaymentCompleted> PaymentCompleted { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;

    public BookingSaga(IHubContext<BookingHub> hubContext)
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreated, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentCompleted, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));

        Initially(
            When(BookingCreated)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.Amount = context.Message.Amount;
                })
                .Send(context => new RequestPayment(context.Saga.BookingId, context.Saga.Amount))
                .TransitionTo(PaymentPending)
        );

        During(PaymentPending,
            When(PaymentCompleted)
                .Send(context => new ConfirmBooking(context.Saga.BookingId))
                .ThenAsync(async context =>
                {
                    await hubContext.Clients.Group(context.Saga.BookingId.ToString())
                        .SendAsync("StatusUpdated", "Confirmed");
                })
                .TransitionTo(Confirmed)
                .Finalize(),
            When(PaymentFailed)
                .Send(context => new CancelBooking(context.Saga.BookingId, context.Message.Reason))
                .ThenAsync(async context =>
                {
                    await hubContext.Clients.Group(context.Saga.BookingId.ToString())
                        .SendAsync("StatusUpdated", "Cancelled");
                })
                .TransitionTo(Cancelled)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
