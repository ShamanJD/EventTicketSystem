namespace EventTicket.Application.Services;

public interface IJwtProvider
{
    public string GenerateToken(Guid userId, string email);
}