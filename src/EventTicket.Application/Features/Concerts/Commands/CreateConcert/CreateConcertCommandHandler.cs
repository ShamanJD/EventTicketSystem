using EventTicket.Application.Services;
using EventTicket.Contracts;
using EventTicket.Domain;
using EventTicket.Domain.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventTicket.Application.Features.Concerts.Commands.CreateConcert;

public class CreateConcertCommandHandler(
    IConcertsDbContext context,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateConcertCommandHandler> logger) : IRequestHandler<CreateConcertCommand, Guid>
{
    public async Task<Guid> Handle(CreateConcertCommand request, CancellationToken cancellationToken)
    {
        var concert = new Concert(request.Name, request.Date, request.Venue);

        if (request.Date < DateTime.Today) throw new InvalidConcertDataException("Date must be in future");

        await context.Concerts.AddAsync(concert, cancellationToken);

        await publishEndpoint.Publish(new ConcertCreatedEvent(concert.Id, concert.Name, concert.Date), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Concert {ConcertName} created successfully with Id {ConcertId}", concert.Name, concert.Id);

        return concert.Id;
    }
}