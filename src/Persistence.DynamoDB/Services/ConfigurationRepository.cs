using Amazon.DynamoDBv2;
using Domain.Entities;
using Newtonsoft.Json;
using Persistence.DynamoDB.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.DynamoDB.Services
{
    [ExcludeFromCodeCoverage]
    [TableName("BookingService_Configuration")]
    public class ConfigurationRepository : DynamoDBTable, IConfigurationRepository
    {
        public ConfigurationRepository(IAmazonDynamoDB database) : base(database)
        {
        }

        public async Task<Configuration?> GetConfigurationAsync()
        {
            var document = await GetItemAsync("CONFIG");
            return document != null
                ? JsonConvert.DeserializeObject<Configuration>(document.ToJson())
                : null;
        }
    }
}
