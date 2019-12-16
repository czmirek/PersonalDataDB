using System.Collections.Generic;

namespace PersonalData.Core
{
    public class TableDefinitionInsertModel : ITableDefinition
    {
        public TableDefinitionInsertModel(string tableId, string? name, string? description, IEnumerable<IColumnDefinition> columns)
        {
            this.ID = tableId ?? throw new System.ArgumentNullException(nameof(tableId));
            this.Name = name;
            this.Description = description;
            this.Columns = columns ?? new IColumnDefinition[] { };
        }

        public string ID { get; private set; }

        public string? Name { get; private set; }

        public string? Description { get; private set; }

        public IEnumerable<IColumnDefinition> Columns { get; private set; }
    }
}