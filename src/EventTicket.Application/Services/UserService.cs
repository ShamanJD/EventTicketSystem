using EventTicket.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventTicket.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Authenticate(string username, string password)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    }
}