using System.Collections.Generic;

namespace PersonalDataDB
{
    public interface ITableDefinition
    {
        string TableName { get; }
        IPrimaryKeyDefinition PrimaryKey { get; }
        IEnumerable<IScalarColumnDefinition> ScalarColumns { get; }
        IEnumerable<IForeignKeyDefinition> ForeignKeys { get; }
        IColumnDefinition this[string columnName] { get; }
        IEnumerable<IColumnDefinition> AllColumns { get; }
    }
}
