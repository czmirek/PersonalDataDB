using System;

namespace PersonalDataDB
{
    public class PersonalDataDBBuilder
    {
        private IDataProvider? dataProvider = null;
        private DefaultTableBuilder dataStructureAndBuilder = new DefaultTableBuilder();
        private Func<ITableSet, IDataProvider>? dataProviderDelegate = null;

        public PersonalDataDB Build()
        {
            if (dataProviderDelegate != null)
                dataProvider = dataProviderDelegate(dataStructureAndBuilder);

            if (dataProvider == null)
                throw new PersonalDataDBException($"Data provider is not specified");

            return new PersonalDataDB(dataProvider, dataStructureAndBuilder);
            
        }

        public PersonalDataDBBuilder UseDataProvider(Func<ITableSet, IDataProvider> dataProviderDelegate)
        {
            this.dataProviderDelegate = dataProviderDelegate;
            return this;
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