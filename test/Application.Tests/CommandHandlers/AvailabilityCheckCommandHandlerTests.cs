using Application.Command;
using Application.CommandHandler;
using Application.Query;
using Domain.Entities;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.CommandHandlers
{
    public class AvailabilityCheckCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly AvailabilityCheckCommandHandler _handler;
        private readonly Configuration _configurationResponse;

        public AvailabilityCheckCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new AvailabilityCheckCommandHandler(_mediator.Object);
            _configurationResponse = new Configuration()
            { MaximumEndReservationDays = 30, MaximumReservationDays = 3, ReservationEndsAt = new TimeSpan(23, 59, 59), ReservationStartsAt = new TimeSpan(0, 0, 0) };

        }

        [Fact]
        public async Task WhenIsAvailable_ShouldReturnIsAvailable()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result.IsAvailable);
        }

        [Fact]
        public async Task WhenIsUpdateAndISAvailable_ShouldReturnIsAvailable()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(1);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            var reservation = new Reservation("user", starts, ends);
            reservationResponse.Add(reservation);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends.AddDays(2), reservation.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result.IsAvailable);
        }

        [Fact]
        public async Task WhenIsUpdateAndIsUnvailable_ShouldReturnIsUnvailable()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(1);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            var reservation = new Reservation("user", starts, ends);
            var reservation2 = new Reservation("user", ends.AddDays(1), ends.AddDays(1));
            reservationResponse.Add(reservation);
            reservationResponse.Add(reservation2);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends.AddDays(1), reservation.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task WhenDateStartIsToday_ShouldReturnError()
        {
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow.AddDays(1);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task WhenEndDateIsEarlyThanStartDate_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(2);
            var ends = DateTime.UtcNow.AddDays(1);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task WhenStartDateIsEarlyThanToday_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(-1);
            var ends = DateTime.UtcNow.AddDays(1);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task WhenStayIsLongerThanConfiguration_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(4);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task WhenEndDateIsAfterPossibleDaysConfigurated_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(29);
            var ends = DateTime.UtcNow.AddDays(30);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync(_configurationResponse);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task WhenConfigurationDoesntLoad_ShouldReturnError()
        {
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow.AddDays(1);
            var reservationResponse = new System.Collections.Generic.List<Domain.Entities.Reservation>();
            _mediator.Setup(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>())).ReturnsAsync((Configuration)null);
            var command = new AvailabilityCheckCommand(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservationsBetween>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(c => c.Send(It.IsAny<GetConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsAvailable);
        }

    }
}