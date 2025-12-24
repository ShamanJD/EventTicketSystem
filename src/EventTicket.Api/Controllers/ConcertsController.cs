using EventTicket.Application.DTOs;
using EventTicket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")] 
public class ConcertsController(IConcertService concertService) : ControllerBase
{
    private IConcertService _concertService = concertService;


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await _concertService.GetConcertByIdAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateConcertDto request, CancellationToken ct)
    {
        var result = await _concertService.CreateConcertAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var concerts = await _concertService.GetAllConcertsAsync();

        var response = concerts.Select(c => new
        {
            id = c.Id,
            name = c.Name,
            date = c.Date.ToString("dd.MM.yyyy")
        });

        return Ok(response);
    }
}