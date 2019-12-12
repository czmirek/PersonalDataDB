namespace PersonalData.Core.Extensions
{
    using System;
    public static class InitializationExtensions
    {
        public static DefaultInitializationDataProvider UseDataManager(this DefaultInitializationDataProvider provider, DataManagerInitialization dataManager)
        {
            DataManagerValidator validator = new DataManagerValidator(DataManagerValidator.ValidationMode.Insert);
            validator.Validate(dataManager);

            provider.DataManager = dataManager;
            return provider;
        }

        public static DefaultInitializationDataProvider UseAdministrator(this DefaultInitializationDataProvider provider, AdministratorInitialization administrator)
        {
            ResponsiblePersonValidator validator = new ResponsiblePersonValidator("Administrator", ResponsiblePersonValidator.ValidationMode.Insert);
            validator.Validate(administrator);

            provider.Administrator = administrator;
            return provider;
        }

        public static DefaultInitializationDataProvider UseConfiguration(this DefaultInitializationDataProvider provider, ConfigurationInitialization configuration)
        {
            provider.Configuration = configuration;
            return provider;
        }

        public static DefaultInitializationDataProvider UseSchema(this DefaultInitializationDataProvider provider, Action<ISchemaBuilder> schemaBuilder)
        {
            var defaultSchemaBuilder = new DefaultSchemaBuilder();
            schemaBuilder.Invoke(defaultSchemaBuilder);
            provider.Schema = defaultSchemaBuilder;
            return provider;
        }
    }
}