using MediatR;

namespace EventTicket.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;

public record LoginResult(bool Success, string? AccessToken = null, string? RefreshToken = null, string? Error = null);
