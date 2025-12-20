namespace EventTicket.Domain.Exceptions;

public class InvalidConcertDataException : DomainException
{
    public InvalidConcertDataException(string message) : base(message)
    {
    }
}