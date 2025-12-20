using EventTicket.Application.DTOs;
using EventTicket.Application.Services;
using EventTicket.Domain;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventTicket.Application.UnitTests;

public class ConcertServiceTests
{
    [Fact]
    public async Task CreateConcertAsync_Should_CallDatabase_And_ReturnDto()
    {
        var dbSetMock = new Mock<DbSet<Concert>>();

        var dbContextMock = new Mock<IApplicationDbContext>();
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        var logger = new Mock<ILogger<ConcertService>>();
        dbContextMock.Setup(m => m.Concerts).Returns(dbSetMock.Object);

        var service = new ConcertService(dbContextMock.Object, publishEndpointMock.Object, logger.Object);

        var dto = new CreateConcertDto("Test Concert", DateTime.UtcNow, "Test Venue");

        var result = await service.CreateConcertAsync(dto, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);

        dbSetMock.Verify(m => m.AddAsync(It.IsAny<Concert>(), It.IsAny<CancellationToken>()), Times.Once);

        dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}