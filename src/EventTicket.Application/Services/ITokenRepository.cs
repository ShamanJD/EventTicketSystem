using EventTicket.Domain;

namespace EventTicket.Application.Services;

public interface ITokenRepository
{
    Task UpdateRefreshToken(string username, string token, CancellationToken ct);
    Task<UserRefreshToken?> GetRefreshToken(string username);
}