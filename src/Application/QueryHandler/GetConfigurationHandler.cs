using Application.Query;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.QueryHandler
{
    public class GetConfigurationHandler : IRequestHandler<GetConfiguration, Configuration>
    {
        private readonly IConfigurationRepository _configurationRepository;

        public GetConfigurationHandler(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<Configuration> Handle(GetConfiguration request, CancellationToken cancellationToken)
        {
            var configuration = await _configurationRepository.GetConfigurationAsync();
            return configuration;
        }
    }
}