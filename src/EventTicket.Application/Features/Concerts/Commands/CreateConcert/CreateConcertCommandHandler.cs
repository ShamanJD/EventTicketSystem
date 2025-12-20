using EventTicket.Application.Services;
using EventTicket.Domain;
using EventTicket.Domain.Exceptions;
using MediatR;

namespace EventTicket.Application.Features.Concerts.Commands.CreateConcert;

public class CreateConcertCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateConcertCommand, Guid>
{
    public async Task<Guid> Handle(CreateConcertCommand request, CancellationToken cancellationToken)
    {
        var concert = new Concert(request.Name, request.Date, request.Venue);

        if (request.Date < DateTime.Today) throw new InvalidConcertDataException("Date must be in future");

        await context.Concerts.AddAsync(concert, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return concert.Id;
    }
}