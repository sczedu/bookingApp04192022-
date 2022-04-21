using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Persistence.DynamoDB.Attributes;

namespace Persistence.DynamoDB
{
    public abstract class DynamoDBTable
    {
        protected IAmazonDynamoDB Database { get; }

        protected DynamoDBTable(IAmazonDynamoDB database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        private string _tableName;

        public string TableName
        {
            get
            {
                if (!string.IsNullOrEmpty(_tableName)) return _tableName;
                var thisType = GetType();
                var tableAttr = (TableName)Attribute.GetCustomAttribute(thisType, typeof(TableName));

                if (tableAttr == null)
                    throw new InvalidOperationException($"No *DynamoTable* attribute defined for {thisType}");

                _tableName = tableAttr.Name;
                return _tableName;
            }
        }

        public virtual Task<Document> GetItemAsync(Primitive hashKey, CancellationToken cancellationToken = default)
        {
            return Table.GetItemAsync(hashKey, cancellationToken);
        }

        public virtual Task<Document> PutItemAsync(Document doc, CancellationToken cancellationToken = default)
        {
            return Table.PutItemAsync(doc, cancellationToken);
        }

        private Table _table;

        protected Table Table
        {
            get
            {
                if (_table != null) return _table;
                _table = Table.LoadTable(Database, TableName);
                return _table;
            }
        }
    }
}