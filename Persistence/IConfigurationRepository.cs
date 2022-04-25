using Domain.Entities;

namespace Persistence
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetConfigurationAsync();
    }
}