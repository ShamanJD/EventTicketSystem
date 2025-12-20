namespace EventTicket.Contracts
{
    public record ConcertCreatedEvent(Guid ConcertId, string Name, DateTime Date);
}
