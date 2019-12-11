namespace PersonalData.Core.Extensions
{
    using System;
    internal class DefaultColumn : IColumnDefinition
    {
        public string ID { get; set; } = String.Empty;
        public ColumnType ColumnType { get; set; }
        public bool IsNullable { get; set; }
        public string? ForeignKeyReferenceTableID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}