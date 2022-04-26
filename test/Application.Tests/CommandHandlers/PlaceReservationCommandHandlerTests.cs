using Application.Command;
using Application.CommandHandler;
using Application.Query;
using Domain.Entities;
using Domain.Entities.Responses;
using MediatR;
using Moq;
using Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.CommandHandlers
{
    public class PlaceReservationCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IReservationRepository> _reservationRepository;
        private readonly PlaceReservationCommandHandler _handler;

        public PlaceReservationCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _reservationRepository = new Mock<IReservationRepository>();
            _handler = new PlaceReservationCommandHandler(_mediator.Object, _reservationRepository.Object);
        }

        [Fact]
        public async Task WhenIsNewReservationAndIsPossible_ShouldPlaceReservation()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse() { IsAvailable = true };

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(true);
            var command = new PlaceReservationCommand(user, starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Once);
            Assert.NotEmpty(result.Id);
        }

        [Fact]
        public async Task WhenIsNewReservationAndGetSomeErrorToPut_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse() { IsAvailable = true };

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(false);
            var command = new PlaceReservationCommand(user, starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Once);
            Assert.Null(result.Id);
        }

        [Fact]
        public async Task WhenIsNewReservationAndDatesAreUnavailable_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse() { 
                IsAvailable = false, Errors = new System.Collections.Generic.List<Error>() { new Error("error") } };

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(true);
            var command = new PlaceReservationCommand(user, starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Never);
            Assert.Null(result.Id);
        }


        [Fact]
        public async Task WhenIsReservationEditAndIsPossible_ShouldEditReservation()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse() { IsAvailable = true };
            var reservation = new Reservation(user, starts, ends);

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservation);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(true);
            var command = new PlaceReservationCommand(starts, ends, reservation.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Once);
            Assert.Equal(reservation.Id, result.Id);
        }

        [Fact]
        public async Task WhenIsReservationEditAndDatesAreUnavailable_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse()
            {
                IsAvailable = false,
                Errors = new System.Collections.Generic.List<Error>() { new Error("error") }
            };
            var reservation = new Reservation(user, starts, ends);

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservation);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(true);
            var command = new PlaceReservationCommand(starts, ends, reservation.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Never);
            Assert.Null(result.Id);
        }

        [Fact]
        public async Task WhenIsReservationEditAndGetFailed_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse() { IsAvailable = true };
            var reservation = (Reservation)null;

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservation);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(true);
            var command = new PlaceReservationCommand(starts, ends, "SomeReservationToEdit");
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Never);
            Assert.Null(result.Id);
        }

        [Fact]
        public async Task WhenIsReservationEditAndIsPossibleButPutGetsError_ShouldReturnError()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var user = "userid";
            var availabilityResponse = new CheckAvailabilityResponse() { IsAvailable = true };
            var reservation = new Reservation(user, starts, ends);

            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(availabilityResponse);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservation);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Reservation>())).ReturnsAsync(false);
            var command = new PlaceReservationCommand(starts, ends, reservation.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Reservation>()), Times.Once);
            Assert.Null(result.Id);
        }
    }
}