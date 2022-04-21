using Domain.Entities;
using MediatR;

namespace Application.Query
{
    public class GetSchedule : IRequest<Schedule>
    {
        public string Id { get; set; }

        public GetSchedule(string id)
        {
            Id = id;
        }
    }
}
