namespace EventTicket.Application.DTOs;

public record ConcertDto(Guid Id, string Name, DateTime Date, string Venue);