namespace EventTicket.Domain;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid ConcertId { get; private set; }
    public decimal Amount { get; private set; }
    public BookingStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public uint RowVersion { get; set; }

    private Booking()
    {
    }

    public Booking(Guid id, Guid concertId, decimal amount)
    {
        Id = id;
        ConcertId = concertId;
        Amount = amount;
        Status = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm booking in status {Status}");

        Status = BookingStatus.Confirmed;
    }

    public void Cancel(string reason)
    {
        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
    }
}
