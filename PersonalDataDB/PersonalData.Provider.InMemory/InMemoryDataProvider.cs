namespace PersonalData.Provider.InMemory
{
    using AutoMapper;
    using PersonalData.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryDataProvider : IDataProvider
    {        
        static InMemoryDataProvider()
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration((cfg) =>
            {
                cfg.CreateMap<IDataManager, DataManagerDataModel>();
                cfg.CreateMap<IAdministrator, AdministratorDataModel>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        private static readonly IMapper mapper;
        private bool initialized = false;
        private Dictionary<Guid, IDataManager> dataManagers = new Dictionary<Guid, IDataManager>();
        private Dictionary<Guid, IResponsiblePerson> administrators = new Dictionary<Guid, IResponsiblePerson>();
        private Dictionary<string, object?> configuration = new Dictionary<string, object?>();
        private Dictionary<string, ITableDefinition> tables = new Dictionary<string, ITableDefinition>();
        public void Initialize()
        {
            initialized = true;
        }

        public bool IsDatabaseInitialized()
        {
            return initialized;
        }
        
        public object InsertDataManager(IDataManager dataManager)
        {
            var newId = Guid.NewGuid();
            
            var dataModel = mapper.Map<DataManagerDataModel>(dataManager);
            dataModel.ID = newId;

            dataManagers.Add(newId, dataModel);
            return newId;
        }

        public IEnumerable<IDataManager> ListDataManagers() => dataManagers.Values;

        public object InsertAdministrator(IAdministrator administrator)
        {
            var newId = Guid.NewGuid();

            var dataModel = mapper.Map<AdministratorDataModel>(administrator);
            dataModel.ID = newId;

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