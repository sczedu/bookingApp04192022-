using MediatR;

namespace Application.Command
{
    public class CancelReservationCommand : IRequest<bool>
    {
        public string ReservationId { get; set; }

        public CancelReservationCommand(string reservationId)
        {
            ReservationId = reservationId;
        }
    }
}
