namespace PersonalData.Provider.InMemory
{
    using PersonalData.Core;
    using System;
    using System.Collections.Generic;

    public class InMemoryDataProvider : IDataProvider
    {        
        private bool initialized = false;
        private Dictionary<Guid, IDataManager> dataManagers = new Dictionary<Guid, IDataManager>();
        private Dictionary<Guid, IResponsiblePerson> administrators = new Dictionary<Guid, IResponsiblePerson>();
        public void Initialize()
        {
            initialized = true;
        }

        public bool IsDatabaseInitialized()
        {
            return initialized;
        }

        
        public object InsertManager(IDataManager dataManager)
        {
            var newId = Guid.NewGuid();
            dataManagers.Add(newId, dataManager);
            return newId;
        }

        public object InsertAdministrator(IResponsiblePerson administrator)
        {
            var newId = Guid.NewGuid();
            administrators.Add(newId, administrator);
            return newId;
        }
    }
}