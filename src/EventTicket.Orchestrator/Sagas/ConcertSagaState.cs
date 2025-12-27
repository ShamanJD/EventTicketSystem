using MassTransit;

namespace EventTicket.Orchestrator.Sagas;

public class ConcertSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }
}
