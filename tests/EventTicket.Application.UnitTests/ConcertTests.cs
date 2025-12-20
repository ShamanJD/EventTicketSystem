using EventTicket.Domain;
using EventTicket.Domain.Exceptions;
using FluentAssertions;
using System.Collections.Generic;

namespace EventTicket.Application.UnitTests
{
    public class ConcertTests
    {
        [Fact] 
        public void Constructor_Should_ThrowException_When_NameIsInvalid()
        {
            string invalidName = "";

            Action action = () => new Concert(invalidName, DateTime.UtcNow, "Venue");

            action.Should().Throw<DomainException>() 
                .WithMessage("*Name*");
        }

        [Fact]
        public void Constructor_Should_CreateConcert_When_DataIsValid()
        {
            string name = "Rock Fest";

            var concert = new Concert(name, DateTime.UtcNow, "Venue");

            concert.Id.Should().NotBeEmpty();
            concert.Name.Should().Be(name);
        }
    }
}
