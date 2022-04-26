using Application.Command;
using Application.Query;
using MediatR;
using Persistence;

namespace Application.CommandHandler
{
    public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IReservationRepository _reservationRepository;

        public CancelReservationCommandHandler(IMediator mediator, IReservationRepository reservationRepository)
        {
            _mediator = mediator;
            _reservationRepository = reservationRepository;
        }

        public async Task<bool> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _mediator.Send(new GetReservation(request.ReservationId));
            if (reservation != null)
            {
                if (reservation.Active)
                {
                    reservation.ReservationDelete();
                    return await _reservationRepository.PutReservationAsync(reservation);
                }
                return true;
            }

            return false;
        }
    }
}
