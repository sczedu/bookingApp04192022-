using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domain.Entities;
using Newtonsoft.Json;
using Persistence.DynamoDB.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.DynamoDB.Services
{
    [ExcludeFromCodeCoverage]
    [TableName("BookingService_Reservation")]
    public class ReservationRepository : DynamoDBTable, IReservationRepository
    {
        public ReservationRepository(IAmazonDynamoDB database) : base(database)
        {
        }

        public async Task<bool> PutReservationAsync(Reservation reservation)
        {
            var jsonText = "";
            try
            {
                jsonText = JsonConvert.SerializeObject(reservation, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                jsonText = jsonText.Replace("true", "\"true\"").Replace("false", "\"false\"");
                var item = Document.FromJson(jsonText);
                var response = await PutItemAsync(item);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(jsonText, ex);
            }

            return false;
        }

        public async Task<Reservation?> GetReservationAsync(string id)
        {
            var document = await GetItemAsync(id);
            return document != null
                ? JsonConvert.DeserializeObject<Reservation>(document.ToJson())
                : null;
        }

        public async Task<IEnumerable<Reservation>> GetReservationsStartsBetweenAsync(DateTime starts, DateTime ends, bool active)
        {
            QueryRequest request = new QueryRequest
            {
                TableName = TableName,
                IndexName = "reservation_starts_index",
                KeyConditionExpression = "#active = :active AND #starts BETWEEN :starts AND :ends",
                ExpressionAttributeNames = new Dictionary<string, string> {
                    { "#active", "Active" },
                    { "#starts", "Starts" },},
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":active", new AttributeValue { S = active.ToString().ToLower() } },
                    { ":starts", new AttributeValue { S = starts.ToString("s") } },
                    { ":ends", new AttributeValue { S = ends.ToString("s") } }},
                ScanIndexForward = true
            };

            QueryResponse queryResult = await Database.QueryAsync(request);

            return queryResult.Items
                .Select(Document.FromAttributeMap)
                .Select(d => d.ToJson())
                .Select(JsonConvert.DeserializeObject<Reservation>);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsEndsBetweenAsync(DateTime starts, DateTime ends, bool active)
        {
            QueryRequest request = new QueryRequest
            {
                TableName = TableName,
                IndexName = "reservation_ends_index",
                KeyConditionExpression = "#active = :active AND #ends BETWEEN :starts AND :ends",
                ExpressionAttributeNames = new Dictionary<string, string> {
                    { "#active", "Active" },
                    { "#ends", "Ends" },},
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":active", new AttributeValue { S = active.ToString().ToLower() } },
                    { ":starts", new AttributeValue { S = starts.ToString("s") } },
                    { ":ends", new AttributeValue { S = ends.ToString("s") } }},
                ScanIndexForward = true
            };

            QueryResponse queryResult = await Database.QueryAsync(request);

            return queryResult.Items
                .Select(Document.FromAttributeMap)
                .Select(d => d.ToJson())
                .Select(JsonConvert.DeserializeObject<Reservation>);
        }
    }
}
