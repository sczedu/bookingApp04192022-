using Domain.Entities.Responses;
using MediatR;

namespace Application.Command
{
    public class AvailabilityCheckCommand : IRequest<CheckAvailabilityResponse>
    {
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public string ReservationDismiss { get; set; }

        public AvailabilityCheckCommand(DateTime starts, DateTime ends, string reservationDismiss = "")
        {
            Starts = starts;
            Ends = ends;
            ReservationDismiss = reservationDismiss;
        }
    }
}
