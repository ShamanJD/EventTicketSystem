using MediatR;

namespace EventTicket.Application.Features.Concerts.Queries.ConcertExists;

public record ConcertExistsQuery(Guid ConcertId) : IRequest<bool>;
