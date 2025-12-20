using MediatR;

namespace EventTicket.Application.Features.Concerts.Commands.CreateConcert
{
    public record CreateConcertCommand(string Name, DateTime Date, string Venue) : IRequest<Guid>;
}
