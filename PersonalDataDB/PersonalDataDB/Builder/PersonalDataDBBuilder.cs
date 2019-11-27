using System;

namespace PersonalDataDB
{
    public class PersonalDataDBBuilder
    {
        private IDataProvider? dataProvider = null;
        private DefaultTableBuilder dataStructureAndBuilder = new DefaultTableBuilder();

        public PersonalDataDB Build()
        {
            if (dataProvider == null)
                throw new PersonalDataDBException($"Data provider is not specified");

            dataProvider.UseStructure(dataStructureAndBuilder.Tables);

            return new PersonalDataDB(dataProvider, dataStructureAndBuilder.Tables);
            
        }

        public PersonalDataDBBuilder UseDataProvider(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            return this;
        }

        public PersonalDataDBBuilder ConfigureTables(Action<ITableBuilder> tableBuilder)
        {
            tableBuilder.Invoke(dataStructureAndBuilder);
            return this;
        }
    }
}