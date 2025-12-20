using EventTicket.Domain.Exceptions;

namespace EventTicket.Domain;

public class Concert
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime Date { get; private set; }
    public string Venue { get; private set; }

    private Concert() { }

    public Concert(string name, DateTime date, string venue)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new InvalidConcertDataException("Name required");

        Id = Guid.NewGuid();
        Name = name;
        Date = date;
        Venue = venue;
    }
}