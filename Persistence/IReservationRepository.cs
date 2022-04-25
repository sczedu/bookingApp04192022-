using Domain.Entities;

namespace Persistence
{
    public interface IReservationRepository
    {
        Task<Reservation> GetReservationAsync(string id);
        Task<IEnumerable<Reservation>> GetReservationsStartsBetweenAsync(DateTime starts, DateTime ends, bool active);
        Task<IEnumerable<Reservation>> GetReservationsEndsBetweenAsync(DateTime starts, DateTime ends, bool active);

        Task<bool> PutReservationAsync(Reservation reservation);

    }
}