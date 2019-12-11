namespace PersonalData.Provider.InMemory
{
    using System;
    using PersonalData.Core;

    internal class AdministratorLogDataModel : IAdministratorLog
    {
        public object? ID { get; set; }

        public DateTime Created { get; set; }

        public object AdministratorId { get; set; } = new object();

        public string Text { get; set; } = String.Empty;
    }
}