namespace Persistence.DynamoDB.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableName : Attribute
    {
        public string Name { get; }

        public TableName(string name)
        {
            Name = name;
        }
    }
}
