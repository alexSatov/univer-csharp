using System;
using System.Linq;
using System.Collections.Generic;

namespace Generics.Tables
{
    public struct TableCell<TRow, TColumn>
    {
        public readonly TRow Row;
        public readonly TColumn Column;

        public TableCell(TRow row, TColumn column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            var other = (TableCell<TRow, TColumn>) obj;
            return Row.Equals(other.Row) && Column.Equals(other.Column);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TRow>.Default.GetHashCode(Row)*397) ^ 
                    EqualityComparer<TColumn>.Default.GetHashCode(Column);
            }
        }
    }

    public class Table<TRow, TColumn, TValue>
    {
        public OpenTable<TRow, TColumn, TValue> Open { get; }
        public ExistedTable<TRow, TColumn, TValue> Existed { get; }
        public IEnumerable<TRow> Rows => rows.Select(r => r);
        public IEnumerable<TColumn> Columns => columns.Select(c => c);

        private readonly HashSet<TRow> rows = new HashSet<TRow>();
        private readonly HashSet<TColumn> columns = new HashSet<TColumn>();
        private readonly Dictionary<TableCell<TRow, TColumn>, TValue> cells = new Dictionary<TableCell<TRow, TColumn>, TValue>();

        public Table()
        {
            Open = new OpenTable<TRow, TColumn, TValue>(this);
            Existed = new ExistedTable<TRow, TColumn, TValue>(this);
        }

        public void AddRow(TRow row)
        {
            rows.Add(row);
        }

        public void AddColumn(TColumn column)
        {
            columns.Add(column);
        }

        public class OpenTable<TRow, TColumn, TValue>
        {
            private readonly Table<TRow, TColumn, TValue> mainTable;

            public OpenTable(Table<TRow, TColumn, TValue> table)
            {
                mainTable = table;
            }

            public TValue this[TRow row, TColumn column]
            {
                get
                {
                    var searchedCell = new TableCell<TRow, TColumn>(row, column);
                    return mainTable.cells.ContainsKey(searchedCell) ? mainTable.cells[searchedCell] : default(TValue);
                }

                set
                {
                    if (!mainTable.rows.Contains(row))
                        mainTable.rows.Add(row);
                    if (!mainTable.columns.Contains(column))
                        mainTable.columns.Add(column);
                    mainTable.cells[new TableCell<TRow, TColumn>(row, column)] = value;
                }
            }
        }

        public class ExistedTable<TRow, TColumn, TValue>
        {
            private readonly Table<TRow, TColumn, TValue> mainTable;

            public ExistedTable(Table<TRow, TColumn, TValue> table)
            {
                mainTable = table;
            }

            public TValue this[TRow row, TColumn column]
            {
                get
                {
                    var searchedCell = new TableCell<TRow, TColumn>(row, column);
                    if (!mainTable.rows.Contains(row) || !mainTable.columns.Contains(column))
                        throw new ArgumentException("Ячейки не существует");
                    return mainTable.cells.ContainsKey(searchedCell) ? mainTable.cells[searchedCell] : default(TValue);
                }

                set
                {
                    if (!mainTable.rows.Contains(row) || !mainTable.columns.Contains(column))
                        throw new ArgumentException("Ячейки не существует");
                    mainTable.cells[new TableCell<TRow, TColumn>(row, column)] = value;
                }
            }
        }
    }
}
