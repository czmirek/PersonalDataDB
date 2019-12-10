namespace PersonalData.Test
{
    using System;
    using System.Collections.Generic;
    using PersonalData.Core;
    public class TableDefinitionMock : ITableDefinition
    {
        public string ID { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;

        public List<ColumnDefinitionMock> ColumnMocks = new List<ColumnDefinitionMock>();
        public IEnumerable<IColumnDefinition> Columns => ColumnMocks;
    }
}