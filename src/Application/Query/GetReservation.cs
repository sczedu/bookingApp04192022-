using Domain.Entities;
using MediatR;

namespace Application.Query
{
    public class GetReservation : IRequest<Reservation>
    {
        public string Id { get; set; }

        public GetReservation(string id)
        {
            Id = id;
        }
    }
}
