using Domain.Entities;
using MediatR;

namespace Application.Query
{
    public class GetReservationsBetween : IRequest<List<Reservation>>
    {
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public bool Active { get; set; }

        public GetReservationsBetween(DateTime starts, DateTime ends, bool active = true)
        {
            Starts = starts;
            Ends = ends;
            Active = active;
        }
    }
}
