using Application.Command;
using Application.Query;
using Domain.Entities;
using Domain.Entities.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.ApiModels.Request;
using Web.Controllers;
using Xunit;

namespace Web.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly BookingController _bookingController;
        public BookingControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _bookingController = new BookingController(_mediator.Object);
        }

        [Fact]
        public async Task GetReservation_WhenReservationExists_ShouldReturnsReservation()
        {
            var request = new Reservation("user", DateTime.UtcNow, DateTime.UtcNow);
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(request);

            var result = await _bookingController.GetReservation("id");
            var okResult = (OkObjectResult)result;
            var response = (Reservation)okResult.Value;

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetReservation_WhenCanNotFound_ShouldReturnsNotFound()
        {
            var request = (Reservation)null;
            _mediator.Setup(c => c.Send(It.IsAny<GetReservation>(), It.IsAny<CancellationToken>())).ReturnsAsync(request);

            var result = await _bookingController.GetReservation("id");
            var okResult = (NotFoundResult)result;

            Assert.Equal(404, okResult.StatusCode);
        }

        [Fact]
        public async Task AvailabilityCheck_WhenCalled_ShouldReturnsResult()
        {
            var request = new CheckAvailabilityResponse();
            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(request);

            var result = await _bookingController.AvailabilityCheck(new AvailabilityCheckRequest());
            var okResult = (OkObjectResult)result;
            var response = (CheckAvailabilityResponse)okResult.Value;

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task AvailabilityCheck_WhenResultIsNull_ShouldReturnsBadRequest()
        {
            var request = (CheckAvailabilityResponse)null;
            _mediator.Setup(c => c.Send(It.IsAny<AvailabilityCheckCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(request);

            var result = await _bookingController.AvailabilityCheck(new AvailabilityCheckRequest());
            var okResult = (BadRequestResult)result;

            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateReservation_WhenIsOk_ShouldReturnsReservationConfirmation()
        {
            var userIdentification = "user";
            var reservationId = "reservationId";
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow;

            var reservationResponse = new ReservationResponse(reservationId, starts, ends);    
            _mediator.Setup(c => c.Send(It.IsAny<PlaceReservationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);

            var reservationRequest = new ReservationRequest() { UserIdentification = userIdentification, Starts = starts, Ends = ends };
            var result = await _bookingController.CreateReservation(reservationRequest);
            var okResult = (ObjectResult)result;
            var response = (ReservationResponse)okResult.Value;

            Assert.Equal(201, okResult.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CreateReservation_WhenUserIsEmpty_ShouldReturnsBadRequest()
        {
            var userIdentification = "";
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow;

            var reservationRequest = new ReservationRequest() { UserIdentification = userIdentification, Starts = starts, Ends = ends };
            var result = await _bookingController.CreateReservation(reservationRequest);
            var okResult = (BadRequestResult)result;

            _mediator.Verify(c => c.Send(It.IsAny<PlaceReservationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateReservation_WhenGetAnyError_ShouldReturnsErrors()
        {
            var userIdentification = "user";
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow;

            var reservationResponse = new ReservationResponse(new Error(""));
            _mediator.Setup(c => c.Send(It.IsAny<PlaceReservationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);

            var reservationRequest = new ReservationRequest() { UserIdentification = userIdentification, Starts = starts, Ends = ends };
            var result = await _bookingController.CreateReservation(reservationRequest);
            var okResult = (ObjectResult)result;
            var response = (ReservationResponse)okResult.Value;

            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task EditReservation_WhenIsOk_ShouldReturnsReservationConfirmation()
        {
            var reservationId = "reservationId";
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow;

            var reservationResponse = new ReservationResponse(reservationId, starts, ends);
            _mediator.Setup(c => c.Send(It.IsAny<PlaceReservationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);

            var reservationRequest = new ModifyReservationRequest() { ReservationId = reservationId, Starts = starts, Ends = ends };
            var result = await _bookingController.EditReservation(reservationRequest);
            var okResult = (ObjectResult)result;
            var response = (ReservationResponse)okResult.Value;

            Assert.Equal(201, okResult.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task EditReservation_WhenReservationIdIsEmpty_ShouldReturnsBadRequest()
        {
            var reservationId = "";
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow;

            var reservationRequest = new ModifyReservationRequest() { ReservationId = reservationId, Starts = starts, Ends = ends };
            var result = await _bookingController.EditReservation(reservationRequest);
            var okResult = (BadRequestResult)result;

            _mediator.Verify(c => c.Send(It.IsAny<PlaceReservationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task EditReservation_WhenGetAnyError_ShouldReturnsErrors()
        {
            var reservationId = "reservationId";
            var starts = DateTime.UtcNow;
            var ends = DateTime.UtcNow;

            var reservationResponse = new ReservationResponse(new Error(""));
            _mediator.Setup(c => c.Send(It.IsAny<PlaceReservationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(reservationResponse);

            var reservationRequest = new ModifyReservationRequest() { ReservationId = reservationId, Starts = starts, Ends = ends };
            var result = await _bookingController.EditReservation(reservationRequest);
            var okResult = (ObjectResult)result;
            var response = (ReservationResponse)okResult.Value;

            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task ReservationCancelAsync_WhenReservationExists_ShouldReturnsReservation()
        {
            _mediator.Setup(c => c.Send(It.IsAny<CancelReservationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _bookingController.ReservationCancelAsync("id");
            var okResult = (OkResult)result;

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ReservationCancelAsync_WhenCanNotFound_ShouldReturnsNotFound()
        {
            _mediator.Setup(c => c.Send(It.IsAny<CancelReservationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _bookingController.ReservationCancelAsync("id");
            var okResult = (NotFoundResult)result;

            Assert.Equal(404, okResult.StatusCode);
        }

        [Fact]
        public async Task ReservationCancelAsync_WhenIdIsEMpty_ShouldReturnsBadRequest()
        {
            var result = await _bookingController.ReservationCancelAsync("");
            var okResult = (BadRequestResult)result;

            _mediator.Verify(c => c.Send(It.IsAny<CancelReservationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal(400, okResult.StatusCode);
        }
    }
}