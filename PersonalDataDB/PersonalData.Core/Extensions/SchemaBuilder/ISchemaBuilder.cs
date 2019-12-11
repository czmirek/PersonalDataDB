namespace PersonalData.Core.Extensions
{
    using System;
    public interface ISchemaBuilder
    {
        void AddTable(string id, Action<ITableBuilder> cb);
    }
}