using Application.Query;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.QueryHandler
{
    public class GetReservationHandler : IRequestHandler<GetReservation, Reservation>
    {
        private readonly IReservationRepository _reservationRepository;

        public GetReservationHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<Reservation> Handle(GetReservation request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                return null;

            var reservation = await _reservationRepository.GetReservationAsync(request.Id);
            return reservation;
        }
    }
}