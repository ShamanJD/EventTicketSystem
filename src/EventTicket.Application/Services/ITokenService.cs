using System.Security.Claims;
using EventTicket.Domain;

namespace EventTicket.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}