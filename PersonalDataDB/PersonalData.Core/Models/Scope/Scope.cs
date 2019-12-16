namespace PersonalData.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    public sealed class Scope
    {
        public ScopeType ScopeType { get; }
        public ReadOnlyCollection<CellScope>? CellIDs { get; private set; }
        public ReadOnlyCollection<RowScope>? RowIDs { get; private set; }
        public IReadOnlyCollection<string>? TableIDs { get; private set; }
        public IReadOnlyCollection<ColumnScope>? ColumnIDs { get; private set; }

        public static Scope DatabaseScope = new Scope();

        internal Scope() 
        {
            ScopeType = ScopeType.Database;
        }

        /// <summary>
        /// Creates a new scope for selected tables
        /// </summary>
        /// <param name="tableIDs">Enumeration of table identifiers</param>
        public Scope(IEnumerable<string> tableIDs)
        {
            if (tableIDs is null)
                throw new ArgumentNullException(nameof(tableIDs));

            if (!tableIDs.Any())
                throw new ArgumentException("Enumeration must not be empty", nameof(tableIDs));

            ScopeType = ScopeType.Tables;
            this.TableIDs = new ReadOnlyCollection<string>(tableIDs.ToList());
        }

        public Scope(IEnumerable<ColumnScope> columnIDs)
        {
            if (columnIDs is null)
                throw new ArgumentNullException(nameof(columnIDs));

            if (!columnIDs.Any())
                throw new ArgumentException("Enumeration must not be empty", nameof(columnIDs));

            ScopeType = ScopeType.Columns;
            this.ColumnIDs = new ReadOnlyCollection<ColumnScope>(columnIDs.ToList());
        }

        public Scope(IEnumerable<RowScope> rowIDs)
        {
            if (rowIDs is null)
                throw new ArgumentNullException(nameof(rowIDs));

            if (!rowIDs.Any())
                throw new ArgumentException("Enumeration must not be empty", nameof(rowIDs));

            ScopeType = ScopeType.Rows;
            this.RowIDs = new ReadOnlyCollection<RowScope>(rowIDs.ToList());
        }

        public Scope(IEnumerable<CellScope> cellIDs)
        {
            if (cellIDs is null)
                throw new ArgumentNullException(nameof(cellIDs));

            if (!cellIDs.Any())
                throw new ArgumentException("Enumeration must not be empty", nameof(cellIDs));
            
            ScopeType = ScopeType.Cells;
            this.CellIDs = new ReadOnlyCollection<CellScope>(cellIDs.ToList());
        }
    }
}