namespace PersonalData.Core
{
    using System;
    internal class OwnerLogInternalModel : IOwnerLog
    {
        public OwnerLogInternalModel(DateTime created, object ownerId, string text)
        {
            Created = created;
            OwnerId = ownerId;
            Text = text;
        }
        object? IOwnerLog.ID => null;

        public DateTime Created { get; set; }

        public object OwnerId { get; set; }

        public string Text { get; set; }
    }
}
