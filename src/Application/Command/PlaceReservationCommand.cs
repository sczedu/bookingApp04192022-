using Domain.Entities.Responses;
using MediatR;

namespace Application.Command
{
    public class PlaceReservationCommand : IRequest<ReservationResponse>
    {
        public string UserIdentification { get; set; }
        public string ReservationId { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }

        public PlaceReservationCommand(string userIdentification, DateTime starts, DateTime ends)
        {
            UserIdentification = userIdentification;
            Starts = starts;
            Ends = ends;
        }

        public PlaceReservationCommand(DateTime starts, DateTime ends, string reservationId)
        {
            Starts = starts;
            Ends = ends;
            ReservationId = reservationId;
        }
    }
}
