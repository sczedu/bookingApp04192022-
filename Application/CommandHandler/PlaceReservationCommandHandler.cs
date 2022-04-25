using Application.Command;
using Application.Query;
using Domain.Entities;
using Domain.Entities.Responses;
using Domain.Enum;
using MediatR;
using Persistence;

namespace Application.CommandHandler
{
    public class PlaceReservationCommandHandler : IRequestHandler<PlaceReservationCommand, ReservationResponse>
    {
        private readonly IMediator _mediator;
        private readonly IReservationRepository _reservationRepository;

        public PlaceReservationCommandHandler(IMediator mediator, IReservationRepository reservationRepository)
        {
            _mediator = mediator;
            _reservationRepository = reservationRepository;
        }

        public async Task<ReservationResponse> Handle(PlaceReservationCommand request, CancellationToken cancellationToken)
        {
            var availability = await _mediator.Send(new AvailabilityCheckCommand(request.Starts, request.Ends, request.ReservationId));

            if (availability.IsAvailable && !availability.Errors.Any())
            {
                var reservation = (Reservation)null;
                if (string.IsNullOrWhiteSpace(request.ReservationId))
                    reservation = new Reservation(request.UserIdentification, availability.Starts, availability.Ends);
                else
                {
                    reservation = await _mediator.Send(new GetReservation(request.ReservationId));
                    if (reservation != null && reservation.Active)
                        reservation.ReservationUpdate(availability.Starts, availability.Ends);
                    else
                        return new ReservationResponse(new Error(AvailabilityProblems.ReservationIsUnavailable));
                }

                var result = await _reservationRepository.PutReservationAsync(reservation);

                if (result)
                    return new ReservationResponse(reservation.Id, reservation.Starts, reservation.Ends);
                else
                    return new ReservationResponse(new Error(AvailabilityProblems.InternalProblem));
            }

            return new ReservationResponse(availability.Errors, availability.Starts, availability.Ends);
        }
    }
}
