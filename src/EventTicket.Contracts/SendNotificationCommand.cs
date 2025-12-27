namespace EventTicket.Contracts;

public record SendNotificationCommand(Guid ConcertId, string Name, DateTime Date);
