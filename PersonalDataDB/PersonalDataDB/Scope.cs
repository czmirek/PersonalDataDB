namespace PersonalDataDB
{
    public class Scope
    {
        private Scope() { }
        private Scope(int ownerID, int? rowID, string table, string column)
        {
            OwnerID = ownerID;
            RowID = RowID;
            Table = table;
            Column = column;
        }

        public static Scope GlobalScope { get; } = new Scope();
        public static Scope CreateOwnerScope(int ownerID) => new Scope(ownerID, null, null, null);
        public static Scope CreateColumnScope(int ownerID, string column) => new Scope(ownerID, null, null, column);
        public static Scope CreateColumnScope(int ownerID, string table, string column) => new Scope(ownerID, null, table, column);
        public static Scope CreateCellScope(int ownerID, string table, string column, int rowID) => new Scope(ownerID, rowID, table, column);

        public int? OwnerID { get; }
        public int? RowID { get; }
        public string Table { get; }
        public string Column { get; }
    }
}