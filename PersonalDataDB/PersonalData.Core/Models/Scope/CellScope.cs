namespace PersonalData.Core
{
    public class CellScope
    {
        public CellScope(string tableID, string rowID, string columnID)
        {
            this.TableID = tableID;
            this.RowID = rowID;
            this.ColumnID = columnID;
        }

        public string TableID { get; }
        public string RowID { get; }
        public string ColumnID { get; }
    }
}