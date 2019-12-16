namespace PersonalData.Core
{
    public sealed class ColumnScope
    {
        public ColumnScope(string tableId, string columnId)
        {
            this.TableId = tableId;
            this.ColumnId = columnId;
        }

        public string TableId { get; }
        public string ColumnId { get; }
    }
}