using Application.Query;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.QueryHandler
{
    public class GetScheduleHandler : IRequestHandler<GetSchedule, Schedule>
    {
        private readonly IScheduleRepository _scheduleRepository;

        public GetScheduleHandler(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<Schedule> Handle(GetSchedule request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetScheduleAsync(request.Id);
            return schedule;
        }
    }
}