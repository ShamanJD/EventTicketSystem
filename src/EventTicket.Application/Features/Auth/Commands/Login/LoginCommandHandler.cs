using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventTicket.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EventTicket.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IAuthDbContext context,
    IConfiguration config) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password, cancellationToken);

        if (user == null)
        {
            return new LoginResult(false, Error: "Invalid credentials");
        }

        var accessToken = GenerateAccessToken(user.Username, user.Id);
        var refreshToken = GenerateRefreshToken();

        await UpdateRefreshToken(user.Username, refreshToken, cancellationToken);

        return new LoginResult(true, accessToken, refreshToken);
    }

    private string GenerateAccessToken(string username, int userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(config["Jwt:AccessTokenExpirationMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task UpdateRefreshToken(string username, string token, CancellationToken ct)
    {
        var expiryDate = DateTime.UtcNow.AddDays(7);

        var existingToken = await context.UserRefreshTokens
            .FirstOrDefaultAsync(t => t.Username == username, ct);

        if (existingToken != null)
        {
            existingToken.Token = token;
            existingToken.ExpiryDate = expiryDate;
        }
        else
        {
            context.UserRefreshTokens.Add(new Domain.UserRefreshToken
            {
                Username = username,
                Token = token,
                ExpiryDate = expiryDate
            });
        }

        await context.SaveChangesAsync(ct);
    }
}
