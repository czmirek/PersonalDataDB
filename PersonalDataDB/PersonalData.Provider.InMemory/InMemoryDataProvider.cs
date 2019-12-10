namespace PersonalData.Provider.InMemory
{
    using PersonalData.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryDataProvider : IDataProvider
    {        
        private bool initialized = false;
        private Dictionary<Guid, IDataManager> dataManagers = new Dictionary<Guid, IDataManager>();
        private Dictionary<Guid, IResponsiblePerson> administrators = new Dictionary<Guid, IResponsiblePerson>();
        private Dictionary<string, object?> configuration = new Dictionary<string, object?>();
        private Dictionary<string, ITableDefinition> tables = new Dictionary<string, ITableDefinition>();
        
        //private Dictionary<string, >
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

        public void SetConfiguration<T>(string key, T value) where T : struct
        {
            configuration[key] = value;
        }

        public T GetConfiguration<T>(string key) where T : struct
        {
            return (T)configuration[key]!;
        }

        public void InitializeSchema(ISchema schema)
        {
            tables = schema.Tables.ToDictionary(t => t.ID);
        }
    }
}