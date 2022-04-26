using Application.Query;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.QueryHandler
{
    public class GetReservationsBetweenHandler : IRequestHandler<GetReservationsBetween, List<Reservation>>
    {
        private readonly IReservationRepository _reservationRepository;

        public GetReservationsBetweenHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<List<Reservation>> Handle(GetReservationsBetween request, CancellationToken cancellationToken)
        {
            var reservationsStartsTask = _reservationRepository.GetReservationsStartsBetweenAsync(request.Starts, request.Ends, request.Active);
            var reservationsEndsTask = _reservationRepository.GetReservationsEndsBetweenAsync(request.Starts, request.Ends, request.Active);

            _ = Task.WhenAll(reservationsStartsTask, reservationsEndsTask);

            var reservationsStarts = reservationsStartsTask.Result;
            var reservationsEnds = reservationsEndsTask.Result;

            var reservationsResult = reservationsStarts?.ToList() ?? new List<Reservation>();
            reservationsResult.AddRange(reservationsEnds?.ToList() ?? new List<Reservation>());
            reservationsResult = reservationsResult.DistinctBy(d => d.Id).ToList();

            return reservationsResult;
        }
    }
}