namespace PersonalData.Core
{
    using System;
    internal class UserLogInternalModel : IUserLog
    {
        public UserLogInternalModel(DateTime created, object userId, string text)
        {
            Created = created;
            UserId = userId;
            Text = text;
        }
        object? IUserLog.ID => null;

        public DateTime Created { get; set; }

        public object UserId { get; set; }

        public string Text { get; set; }
    }
}
