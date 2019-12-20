namespace PersonalData.Core.Services
{
    internal class LockProvider
    {
        private static readonly object dataAccessLock = new object();

        public object GetLock() => dataAccessLock;
    }
}
