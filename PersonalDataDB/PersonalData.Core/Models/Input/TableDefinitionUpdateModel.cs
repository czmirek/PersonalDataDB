namespace PersonalData.Core
{
    using System.Collections.Generic;
    using System.Linq;
    public class TableDefinitionUpdateModel : ITableDefinition
    {
        public TableDefinitionUpdateModel(string tableId, string? name, string? description)
        {
            this.ID = tableId ?? throw new System.ArgumentNullException(nameof(tableId));
            this.Name = name;
            this.Description = description;
        }

        public string ID { get; private set; }

        public string? Name { get; private set; }

        public string? Description { get; private set; }

        IEnumerable<IColumnDefinition> ITableDefinition.Columns => Enumerable.Empty<IColumnDefinition>();
    }
}