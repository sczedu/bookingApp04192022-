using Application.Query;
using Application.QueryHandler;
using Domain.Entities;
using Moq;
using Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.QueryHandlers
{
    public class GetReservationsBetweenHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepository;
        private readonly GetReservationsBetweenHandler _handler;

        public GetReservationsBetweenHandlerTests()
        {
            _reservationRepository = new Mock<IReservationRepository>();
            _handler = new GetReservationsBetweenHandler(_reservationRepository.Object);
        }

        [Fact]
        public async Task WhenIsReservedDates_ShouldReturnsReservations()
        {
            var active = true;
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservation1 = new List<Reservation>() { new Reservation("", starts, ends) };
            var reservation2 = new List<Reservation>() { new Reservation("", starts, ends) };
            _reservationRepository.Setup(c => c.GetReservationsStartsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            _reservationRepository.Setup(c => c.GetReservationsEndsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation2);
            var command = new GetReservationsBetween(starts,ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _reservationRepository.Verify(c => c.GetReservationsStartsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);
            _reservationRepository.Verify(c => c.GetReservationsEndsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task WhenIsReservedDatesAndIsTheSameStartAndEnds_ShouldReturnsTheReservation()
        {
            var active = true;
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservation1 = new List<Reservation>() { new Reservation("", starts, ends) };
            _reservationRepository.Setup(c => c.GetReservationsStartsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            _reservationRepository.Setup(c => c.GetReservationsEndsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            var command = new GetReservationsBetween(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _reservationRepository.Verify(c => c.GetReservationsStartsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);
            _reservationRepository.Verify(c => c.GetReservationsEndsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);

            Assert.Equal(1, result.Count);
        }

        [Fact]
        public async Task WhenTasksFailed_ShouldReturnsEmpty()
        {
            var active = true;
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservation1 = (List<Reservation>)null;
            _reservationRepository.Setup(c => c.GetReservationsStartsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            _reservationRepository.Setup(c => c.GetReservationsEndsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            var command = new GetReservationsBetween(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _reservationRepository.Verify(c => c.GetReservationsStartsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);
            _reservationRepository.Verify(c => c.GetReservationsEndsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);

            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task WhenReturnEmptyLists_ShouldReturnsEmpty()
        {
            var active = true;
            var starts = DateTime.UtcNow.AddDays(1);
            var ends = DateTime.UtcNow.AddDays(3);
            var reservation1 = new List<Reservation>();
            _reservationRepository.Setup(c => c.GetReservationsStartsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            _reservationRepository.Setup(c => c.GetReservationsEndsBetweenAsync(starts, ends, active)).ReturnsAsync(reservation1);
            var command = new GetReservationsBetween(starts, ends);
            var result = await _handler.Handle(command, CancellationToken.None);

            _reservationRepository.Verify(c => c.GetReservationsStartsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);
            _reservationRepository.Verify(c => c.GetReservationsEndsBetweenAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Once);

            Assert.Equal(0, result.Count);
        }

    }
}