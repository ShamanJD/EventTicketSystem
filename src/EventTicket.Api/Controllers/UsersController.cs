using EventTicket.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EventTicket.Api.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class UsersController(IJwtProvider jwtProvider) : ControllerBase
{
    private IJwtProvider _jwtProvider = jwtProvider;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request) 
    {
        if (request.Email == "admin@admin.com" && request.Password == "admin")
        {
            var token = _jwtProvider.GenerateToken(Guid.NewGuid(), request.Email);
            return Ok(new { token });
        }

        return Unauthorized();
    }
}