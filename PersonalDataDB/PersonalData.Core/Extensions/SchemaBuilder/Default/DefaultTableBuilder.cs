namespace PersonalData.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    internal class DefaultTableBuilder : ITableBuilder, ITableDefinition
    {
        public string ID { get; set; } = String.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<IColumnDefinition> Columns => columns.Values;

        private Dictionary<string, DefaultColumn> columns = new Dictionary<string, DefaultColumn>();
        public void AddColumn(string id, ColumnType columnType, bool isNullable, string? name = null, string? description = null)
        {
            if (String.IsNullOrEmpty(id))
                throw new InitializationException($"Column {id} must not be empty");

            if (columns.ContainsKey(id))
                throw new InitializationException($"Column {id} already exists");

            columns.Add(id, new DefaultColumn()
            {
                ID = id,
                ColumnType = columnType,
                IsNullable = isNullable,
                Name = name,
                Description = description
            });
        }
        public void AddForeignKeyColumn(string id, string referencedTableName, bool isNullable, string? name = null, string? description = null)
        {
            if (String.IsNullOrEmpty(id))
                throw new InitializationException($"Column {id} must not be empty");

            if (columns.ContainsKey(id))
                throw new InitializationException($"Column {id} already exists");

            columns.Add(id, new DefaultColumn()
            {
                ID = id,
                ColumnType = ColumnType.ForeignKey,
                IsNullable = isNullable,
                ForeignKeyReferenceTableID = referencedTableName,
                Name = name,
                Description = description
            });
        }
    }
}