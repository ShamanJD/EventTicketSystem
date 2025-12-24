using EventTicket.Api.ViewModels;
using EventTicket.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController (IUserService userService, ITokenService tokenService, ITokenRepository tokenRepository) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken ct)
    {
        var user = await userService.Authenticate(model.Username, model.Password);
        if (user == null) return Unauthorized();

        var accessToken = tokenService.GenerateAccessToken(user);

        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenRepository.UpdateRefreshToken(user.Username, refreshToken, ct);

        return Ok(new { accessToken, refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenApiModel model, CancellationToken ct)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
        var username = principal.Identity.Name;

        var savedRefreshToken = await tokenRepository.GetRefreshToken(username);

        if (savedRefreshToken == null ||
            savedRefreshToken.Token != model.RefreshToken ||
            savedRefreshToken.ExpiryDate <= DateTime.Now)
            return BadRequest("Invalid refresh token");

        var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await tokenRepository.UpdateRefreshToken(username, newRefreshToken, ct);

        return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
    }
}