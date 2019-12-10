namespace PersonalData.Core
{
    using System;
    public static class InitializationExtensions
    {
        public static InitializationDataBuilder ConfigureDataManager(this InitializationDataBuilder builder, Action<DataManagerInitialization> dataManagerSetup)
        {
            DataManagerInitialization dataManagerInitialization = new DataManagerInitialization();
            dataManagerSetup(dataManagerInitialization);
            return builder;
        }

        public static InitializationDataBuilder ConfigureAdministrator(this InitializationDataBuilder builder, Action<AdministratorInitialization> adminSetup)
        {
            AdministratorInitialization adminInit = new AdministratorInitialization();
            adminSetup(adminInit);
            return builder;
        }
    }
}