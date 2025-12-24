using EventTicket.Domain;

namespace EventTicket.Application.Services;

public interface IUserService
{
    Task<User?> Authenticate(string username, string password);
}