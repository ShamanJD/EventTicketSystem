using EventTicket.Application.DTOs;
using MediatR;

namespace EventTicket.Application.Features.Concerts.Queries.GetConcertById;

public record GetConcertByIdQuery(Guid Id) : IRequest<ConcertDto?>;
