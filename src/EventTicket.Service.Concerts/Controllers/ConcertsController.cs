using EventTicket.Application.DTOs;
using EventTicket.Application.Features.Concerts.Commands.CreateConcert;
using EventTicket.Application.Features.Concerts.Queries.GetAllConcerts;
using EventTicket.Application.Features.Concerts.Queries.GetConcertById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Service.Concerts.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConcertsController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetConcertByIdQuery(id), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateConcertDto request, CancellationToken ct)
    {
        var concertId = await mediator.Send(new CreateConcertCommand(request.Name, request.Date, request.Venue), ct);
        return CreatedAtAction(nameof(Get), new { id = concertId }, new { Id = concertId });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var concerts = await mediator.Send(new GetAllConcertsQuery(), ct);

        var response = concerts.Select(c => new
        {
            id = c.Id,
            name = c.Name,
            date = c.Date.ToString("dd.MM.yyyy")
        });

        return Ok(response);
    }
}