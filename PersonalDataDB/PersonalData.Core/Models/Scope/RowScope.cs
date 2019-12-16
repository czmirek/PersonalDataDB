namespace PersonalData.Core
{
    public sealed class RowScope
    {
        public RowScope(string tableID, string rowID)
        {
            this.TableID = tableID;
            this.RowID = rowID;
        }

        public string TableID { get; }
        public string RowID { get; }
    }
}