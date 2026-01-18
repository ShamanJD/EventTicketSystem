using EventTicket.Application.DTOs;
using MediatR;

namespace EventTicket.Application.Features.Concerts.Queries.GetAllConcerts;

public record GetAllConcertsQuery : IRequest<List<ConcertDto>>;
