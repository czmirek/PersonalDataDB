namespace PersonalData.Test
{
    using PersonalData.Core;
    using System;

    public class ColumnDefinitionMock : IColumnDefinition
    {
        public string ID { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public ColumnType ColumnType { get; set; } = ColumnType.String;
        public bool IsNullable { get; set; }
        public string? ForeignKeyReferenceTableID { get; set; }
    }
}