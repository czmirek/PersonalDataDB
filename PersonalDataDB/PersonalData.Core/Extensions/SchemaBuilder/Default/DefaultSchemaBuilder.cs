namespace PersonalData.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    internal class DefaultSchemaBuilder : ISchemaBuilder, ISchema
    {
        private Dictionary<string, DefaultTableBuilder> tables = new Dictionary<string, DefaultTableBuilder>();
        public IEnumerable<ITableDefinition> Tables => tables.Values;

        public void AddTable(string id, Action<ITableBuilder> columnBuilder)
        {
            if (tables.ContainsKey(id))
                throw new InitializationException($"Duplicate table \"{id}\"");

            if (String.IsNullOrWhiteSpace(id))
                throw new InitializationException($"Table \"{id}\" must not be empty or contain whitespace.");

            DefaultTableBuilder newTable = new DefaultTableBuilder() { ID = id };
            columnBuilder.Invoke(newTable);

            tables.Add(id, newTable);
        }
    }
}
