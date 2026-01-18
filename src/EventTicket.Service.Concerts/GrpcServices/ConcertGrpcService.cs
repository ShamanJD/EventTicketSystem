using EventTicket.Application.Features.Concerts.Queries.ConcertExists;
using EventTicket.Application.Features.Concerts.Queries.GetConcertById;
using EventTicket.Contracts.Protos;
using Grpc.Core;
using MediatR;

namespace EventTicket.Service.Concerts.GrpcServices;

public class ConcertGrpcService(IMediator mediator) : ConcertGrpc.ConcertGrpcBase
{
    public override async Task<ConcertExistsResponse> ConcertExists(ConcertExistsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ConcertId, out var concertId))
        {
            return new ConcertExistsResponse { Exists = false };
        }

        var exists = await mediator.Send(new ConcertExistsQuery(concertId), context.CancellationToken);
        
        return new ConcertExistsResponse { Exists = exists };
    }

    public override async Task<GetConcertResponse> GetConcert(GetConcertRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ConcertId, out var concertId))
        {
            return new GetConcertResponse { Found = false };
        }

        var concert = await mediator.Send(new GetConcertByIdQuery(concertId), context.CancellationToken);
        
        if (concert is null)
        {
            return new GetConcertResponse { Found = false };
        }

        return new GetConcertResponse
        {
            Found = true,
            Id = concert.Id.ToString(),
            Name = concert.Name,
            Date = concert.Date.ToString("O"),
            Venue = concert.Venue
        };
    }
}
