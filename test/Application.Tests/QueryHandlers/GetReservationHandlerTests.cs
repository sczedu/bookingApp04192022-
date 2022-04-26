using Application.Query;
using Application.QueryHandler;
using Domain.Entities;
using Moq;
using Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.QueryHandlers
{
    public class GetReservationHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepository;
        private readonly GetReservationHandler _handler;

        public GetReservationHandlerTests()
        {
            _reservationRepository = new Mock<IReservationRepository>();
            _handler = new GetReservationHandler(_reservationRepository.Object);
        }

        [Fact]
        public async Task WhenCalled_ShouldReturnConfiguration()
        {
            var identification = "id";
            var reservation = new Reservation("", DateTime.UtcNow, DateTime.UtcNow);
            _reservationRepository.Setup(c => c.GetReservationAsync(It.IsAny<string>())).ReturnsAsync(reservation);
            var command = new GetReservation(identification);
            var result = await _handler.Handle(command, CancellationToken.None);

            _reservationRepository.Verify(c => c.GetReservationAsync(It.IsAny<string>()), Times.Once);
            Assert.Equal(reservation, result);
        }

        [Fact]
        public async Task WhenIdIsEmpty_ShouldReturnNull()
        {
            var identification = "";
            var command = new GetReservation(identification);
            var result = await _handler.Handle(command, CancellationToken.None);

            _reservationRepository.Verify(c => c.GetReservationAsync(It.IsAny<string>()), Times.Never);
            Assert.Null(result);
        }
    }
}