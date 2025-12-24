using EventTicket.Application.DTOs;

namespace EventTicket.Application.Services;

public interface IConcertService
{
    Task<ConcertDto> CreateConcertAsync(CreateConcertDto dto, CancellationToken cancellationToken);
    Task<ConcertDto?> GetConcertByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ConcertDto>> GetAllConcertsAsync();
}