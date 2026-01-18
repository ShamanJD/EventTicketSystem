using EventTicket.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace EventTicket.Service.Concerts.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is DomainException domainException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new { error = domainException.Message }, cancellationToken);
            return true; 
        }

        return false; 
    }
}
