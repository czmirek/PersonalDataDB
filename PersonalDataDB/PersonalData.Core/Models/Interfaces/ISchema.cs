namespace PersonalData.Core
{
    using System.Collections.Generic;
    public interface ISchema
    {
        public IEnumerable<ITableDefinition> Tables { get; }
    }
}