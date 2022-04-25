using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Newtonsoft.Json;
using Persistence.DynamoDB.Attributes;

namespace Persistence.DynamoDB.Services
{
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
