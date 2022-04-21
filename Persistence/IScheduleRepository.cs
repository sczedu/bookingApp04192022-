using Domain.Entities;

namespace Persistence
{
    public interface IScheduleRepository
    {
        Task<Schedule> GetScheduleAsync(string id);
    }
}