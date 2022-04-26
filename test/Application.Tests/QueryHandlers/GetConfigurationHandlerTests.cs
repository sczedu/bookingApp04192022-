using Application.Query;
using Application.QueryHandler;
using Domain.Entities;
using Moq;
using Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.QueryHandlers
{
    public class GetConfigurationHandlerTests
    {
        private readonly Mock<IConfigurationRepository> _configurationRepository;
        private readonly GetConfigurationHandler _handler;

        public GetConfigurationHandlerTests()
        {
            _configurationRepository = new Mock<IConfigurationRepository>();
            _handler = new GetConfigurationHandler(_configurationRepository.Object);
        }

        [Fact]
        public async Task WhenCalled_ShouldReturnConfiguration()
        {
            var configuration = new Configuration();
            _configurationRepository.Setup(c => c.GetConfigurationAsync()).ReturnsAsync(configuration);
            var command = new GetConfiguration();
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.Equal(configuration, result);
        }
    }
}