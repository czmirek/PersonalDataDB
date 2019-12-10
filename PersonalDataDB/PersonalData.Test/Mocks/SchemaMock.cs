using System.Collections.Generic;
using PersonalData.Core;

namespace PersonalData.Test
{
    internal class SchemaMock : ISchema
    {
        public List<TableDefinitionMock> TableMocks = new List<TableDefinitionMock>();
        public IEnumerable<ITableDefinition> Tables => TableMocks;
    }
}