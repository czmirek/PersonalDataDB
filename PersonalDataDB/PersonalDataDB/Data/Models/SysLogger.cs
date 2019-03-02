namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;
    using System.Data;

    internal class SysLogger
    {
        private readonly IDbConnection db;
        private readonly IDateTimeProvider dateTimeProvider;

        public SysLogger(IDbConnection db, IDateTimeProvider dateTimeProvider)
        {
            this.db = db ?? throw new System.ArgumentNullException(nameof(db));
            this.dateTimeProvider = dateTimeProvider ?? throw new System.ArgumentNullException(nameof(dateTimeProvider));
        }

        public void Log(string text)
        {
            db.Insert(new SystemLog()
            {
                Time = dateTimeProvider.Now,
                Log = text
            });
        }
    }
}
