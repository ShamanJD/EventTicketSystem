using MassTransit;

namespace EventTicket.Orchestrator.Sagas;

public class BookingSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
}
