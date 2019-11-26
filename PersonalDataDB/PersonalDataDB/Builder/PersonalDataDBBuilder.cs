using System;

namespace PersonalDataDB
{
    public class PersonalDataDBBuilder
    {
        private IDataProvider? dataProvider = null;
        private DefaultTableBuilder defaultTableBuilder = new DefaultTableBuilder();

        public PersonalDataDB Build()
        {
            DefaultTableBuilderValidator validator = new DefaultTableBuilderValidator();
            validator.Validate(defaultTableBuilder);
        }

        public PersonalDataDBBuilder UseDataProvider(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            return this;
        }

        public PersonalDataDBBuilder ConfigureTables(Action<ITableBuilder> tableBuilder)
        {
            tableBuilder.Invoke(defaultTableBuilder);
            return this;
        }
    }
}