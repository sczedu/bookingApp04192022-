using Amazon.DynamoDBv2;
using Domain.Entities;
using Newtonsoft.Json;
using Persistence.DynamoDB.Attributes;

namespace Persistence.DynamoDB.Services
{
    [TableName("Booking_Schedules")]
    public class ScheduleRepository : DynamoDBTable, IScheduleRepository
    {
        public ScheduleRepository(IAmazonDynamoDB database) : base(database)
        {
        }

        public async Task<Schedule?> GetScheduleAsync(string id)
        {
            var document = await GetItemAsync(id);
            return document != null
                ? JsonConvert.DeserializeObject<Schedule>(document.ToJson())
                : null;
        }
    }
}
