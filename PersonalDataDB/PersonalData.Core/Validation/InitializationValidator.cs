namespace PersonalData.Core
{
    internal class InitializationValidator
    {
        public void Validate(IInitializationDataProvider initDataProvider)
        {
            if (initDataProvider.DataManager == null)
                throw new PersonalDataDBException($"Data manager must not be null");

            DataManagerValidator dataManagerValidator = new DataManagerValidator();
            dataManagerValidator.Validate(initDataProvider.DataManager);

            if (initDataProvider.Administrator == null)
                throw new PersonalDataDBException($"Administrator must not be null");

            ResponsiblePersonValidator adminValidator = new ResponsiblePersonValidator("Administrator");
            adminValidator.Validate(initDataProvider.Administrator);
        }
    }
}
