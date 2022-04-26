using Application.Command;
using Application.CommandHandler;
using Application.Query;
using MediatR;
using Moq;
using Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.CommandHandlers
{
    public class CancelReservationCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IReservationRepository> _reservationRepository;
        private readonly CancelReservationCommandHandler _handler;

        public CancelReservationCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _reservationRepository = new Mock<IReservationRepository>();
            _handler = new CancelReservationCommandHandler(_mediator.Object, _reservationRepository.Object);
        }

        [Fact]
        public async Task WhenIsPossible_ShouldReturnTrue()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservationResponse = new Domain.Entities.Reservation("userid", starts, ends);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>())).ReturnsAsync(true);
            var command = new CancelReservationCommand(reservationResponse.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task WhenIsAlreadyCanceled_ShouldReturnTrue()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservationResponse = new Domain.Entities.Reservation("userid", starts, ends);
            reservationResponse.Active = false;
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>())).ReturnsAsync(true);
            var command = new CancelReservationCommand(reservationResponse.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Never);
            Assert.True(result);
        }

        [Fact]
        public async Task WhenIsPossibleButGetErrorToUpdateItem_ShouldReturnFalse()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservationResponse = new Domain.Entities.Reservation("userid", starts, ends);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            _reservationRepository.Setup(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>())).ReturnsAsync(false);
            var command = new CancelReservationCommand(reservationResponse.Id);
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Once);
            Assert.False(result);
        }

        [Fact]
        public async Task WhenReservationDoesntExists_ShouldReturnFalse()
        {
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservationResponse = (Domain.Entities.Reservation)null;
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);
            var command = new CancelReservationCommand("reservation123");
            var result = await _handler.Handle(command, CancellationToken.None);

            _mediator.Verify(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepository.Verify(c => c.PutReservationAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Never);
            Assert.False(result);
        }
    }
}