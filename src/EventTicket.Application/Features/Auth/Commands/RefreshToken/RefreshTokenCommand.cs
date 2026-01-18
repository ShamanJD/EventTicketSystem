using MediatR;

namespace EventTicket.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<RefreshTokenResult>;

public record RefreshTokenResult(bool Success, string? AccessToken = null, string? RefreshToken = null, string? Error = null);
