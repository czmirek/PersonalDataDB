using System;

namespace PersonalData.Core
{
    public class ColumnDefinitionUpdateModel : IColumnDefinition
    {
        public ColumnDefinitionUpdateModel(string id, string? name, string? description, bool isNullable)
        {
            this.ID = id;
            this.Name = name;
            this.Description = description;
            this.IsNullable = isNullable;
        }

        public string ID { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsNullable { get; private set; }
        public ColumnType ColumnType => throw new InvalidOperationException();
        public string? ForeignKeyReferenceTableID => null;
    }
}