namespace PersonalData.Core
{
    using System;
    public class ColumnDefinitionInsertModel : IColumnDefinition
    {
        public ColumnDefinitionInsertModel(string id, string? name, string? description, ColumnType columnType, bool isNullable, string? foreignKeyReferenceTableID = null)
        {
            this.ID = id;
            this.Name = name;
            this.Description = description;
            this.ColumnType = columnType;
            this.IsNullable = isNullable;
            this.ForeignKeyReferenceTableID = foreignKeyReferenceTableID;
        }

        public string ID { get; private set; }
        public string? Name { get; private set; }

        public string? Description { get; private set; }

        public ColumnType ColumnType { get; private set; }

        public bool IsNullable { get; private set; }

        public string? ForeignKeyReferenceTableID { get; private set; }
    }
}