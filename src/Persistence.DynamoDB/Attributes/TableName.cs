using System.Diagnostics.CodeAnalysis;

namespace Persistence.DynamoDB.Attributes
{
    [ExcludeFromCodeCoverage]
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
