using System;

namespace PersonalDataDB
{
    public interface ITableBuilder
    {
        void Add(string tableName, Action<IColumnBuilder> cb);
    }
}