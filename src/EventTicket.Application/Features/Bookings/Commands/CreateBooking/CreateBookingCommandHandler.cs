using EventTicket.Contracts.Protos;
using EventTicket.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventTicket.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(
    ConcertGrpc.ConcertGrpcClient concertClient,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateBookingCommandHandler> logger) : IRequestHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<CreateBookingResult> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var concertResponse = await concertClient.GetConcertAsync(
            new GetConcertRequest { ConcertId = request.ConcertId.ToString() },
            cancellationToken: cancellationToken);

        if (!concertResponse.Found)
        {
            logger.LogWarning("Concert {ConcertId} not found via gRPC", request.ConcertId);
            return new CreateBookingResult(false, Error: "Concert not found");
        }

        if (DateTime.TryParse(concertResponse.Date, out var concertDate) && concertDate < DateTime.UtcNow)
        {
            logger.LogWarning("Cannot book tickets for past concert {ConcertId}", request.ConcertId);
            return new CreateBookingResult(false, Error: "Concert has already passed");
        }

        var bookingId = Guid.NewGuid();

        logger.LogInformation("Creating booking {BookingId} for concert {ConcertId} with amount {Amount}",
            bookingId, request.ConcertId, request.Amount);

        await publishEndpoint.Publish(new BookingCreated(bookingId, request.ConcertId, request.Amount), cancellationToken);

        return new CreateBookingResult(true, bookingId);
    }
}
