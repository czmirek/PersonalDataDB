namespace PersonalData.Core
{
    internal class InitializationValidator
    {
        public void Validate(IInitializationDataProvider initDataProvider)
        {
            if (initDataProvider.DataManager == null)
                throw new InitializationException($"Data manager must be set");

            DataManagerValidator dataManagerValidator = new DataManagerValidator();
            dataManagerValidator.Validate(initDataProvider.DataManager);

            if (initDataProvider.Administrator == null)
                throw new InitializationException($"Administrator must be set");

            ResponsiblePersonValidator adminValidator = new ResponsiblePersonValidator("Administrator");
            adminValidator.Validate(initDataProvider.Administrator);

            if(initDataProvider.Configuration == null)
                throw new InitializationException($"Configuration must be set");

            if (initDataProvider.Schema == null)
                throw new InitializationException($"Schema must be set");

            SchemaValidator schemaValidator = new SchemaValidator();
            schemaValidator.Validate(initDataProvider.Schema);
        }
    }
}
