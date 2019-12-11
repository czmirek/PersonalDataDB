namespace PersonalData.Core
{
    using System;
    internal class AdministratorLogInternalModel : IAdministratorLog
    {
        public AdministratorLogInternalModel(DateTime created, object administratorId, string text)
        {
            Created = created;
            AdministratorId = administratorId;
            Text = text;
        }
        object? IAdministratorLog.ID => null;

        public DateTime Created { get; set; }

        public object AdministratorId { get; set; }

        public string Text { get; set; }
    }
}
