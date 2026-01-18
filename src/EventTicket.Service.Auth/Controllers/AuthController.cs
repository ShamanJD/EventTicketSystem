using EventTicket.Application.Features.Auth.Commands.Login;
using EventTicket.Application.Features.Auth.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Service.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(model.Username, model.Password), ct);

        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model, CancellationToken ct)
    {
        var result = await mediator.Send(new RefreshTokenCommand(model.AccessToken, model.RefreshToken), ct);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken });
    }
}

public record LoginRequest(string Username, string Password);
public record RefreshRequest(string AccessToken, string RefreshToken);