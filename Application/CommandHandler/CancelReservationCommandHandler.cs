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
            if (reservation != null && reservation.Active)
            {
                reservation.ReservationDelete();
                var result = await _reservationRepository.PutReservationAsync(reservation);
                if (result)
                    return true;
            }

            return false;
        }
    }
}
