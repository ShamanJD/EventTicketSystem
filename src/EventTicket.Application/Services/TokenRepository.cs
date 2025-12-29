using EventTicket.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventTicket.Application.Services;

public class TokenRepository : ITokenRepository
{
    private readonly IAuthDbContext _context;

    public TokenRepository(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task UpdateRefreshToken(string username, string token, CancellationToken ct)
    {
        var expiryDate = DateTime.UtcNow.AddDays(7);

        var existingToken = await _context.UserRefreshTokens
            .FirstOrDefaultAsync(t => t.Username == username);

        if (existingToken != null)
        {
            existingToken.Token = token;
            existingToken.ExpiryDate = expiryDate;
        }
        else
        {
            _context.UserRefreshTokens.Add(new UserRefreshToken
            {
                Username = username,
                Token = token,
                ExpiryDate = expiryDate
            });
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<UserRefreshToken?> GetRefreshToken(string username)
    {
        return await _context.UserRefreshTokens
            .FirstOrDefaultAsync(t => t.Username == username);
    }
}