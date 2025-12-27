namespace EventTicket.Contracts;

// Events
public record BookingCreated(Guid BookingId, Guid ConcertId, decimal Amount);
public record PaymentCompleted(Guid BookingId);
public record PaymentFailed(Guid BookingId, string Reason);

// Commands
public record RequestPayment(Guid BookingId, decimal Amount);
public record ConfirmBooking(Guid BookingId);
public record CancelBooking(Guid BookingId, string Reason);
