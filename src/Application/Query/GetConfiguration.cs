using Domain.Entities;
using MediatR;

namespace Application.Query
{
    public class GetConfiguration : IRequest<Configuration>
    {

        public GetConfiguration()
        {
        }
    }
}
