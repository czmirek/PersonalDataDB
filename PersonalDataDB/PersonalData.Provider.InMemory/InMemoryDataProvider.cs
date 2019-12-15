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
                cfg.CreateMap<IUser, UserDataModel>();
                cfg.CreateMap<IDataManager, DataManagerDataModel>();
                cfg.CreateMap<IAdministrator, AdministratorDataModel>();
                cfg.CreateMap<IAdministratorLog, AdministratorLogDataModel>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        private static readonly IMapper mapper;
        private bool initialized = false;
        private Dictionary<Guid, IDataManager> dataManagers = new Dictionary<Guid, IDataManager>();
        private Dictionary<Guid, IAdministrator> administrators = new Dictionary<Guid, IAdministrator>();
        private readonly Dictionary<string, object?> configuration = new Dictionary<string, object?>();
        private Dictionary<string, ITableDefinition> tables = new Dictionary<string, ITableDefinition>();
        private Dictionary<Guid, IAdministratorLog> administratorLog = new Dictionary<Guid, IAdministratorLog>();
        private Dictionary<Guid, IUser> users = new Dictionary<Guid, IUser>();
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

            administrators.Add(newId, dataModel);
            return newId;
        }
        public IEnumerable<IAdministratorLog> ListAdministratorLogs() => administratorLog.Values;
        public void InsertAdministratorLog(IAdministratorLog newLog)
        {
            var newId = Guid.NewGuid();

            var dataModel = mapper.Map<AdministratorLogDataModel>(newLog);
            dataModel.ID = newId;

            administratorLog.Add(newId, dataModel);
        }
        public bool CheckAdministratorId(object administratorId)
        {
            if(administratorId is Guid adminGuid)
                return administrators.ContainsKey(adminGuid);

            return false;
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

        public IEnumerable<IAdministrator> ListAdministrators() => administrators.Values;

        public bool CheckDataManagerId(object dataManagerId)
        {
            if (dataManagerId is Guid dataManagerGuid)
                return dataManagers.ContainsKey(dataManagerGuid);

            return false;
        }

        public void UpdateAdministrator(IAdministrator updatedAdministrator)
        {
            Guid id = (Guid)updatedAdministrator.ID;
            var model = mapper.Map<AdministratorDataModel>(administrators[id]);
            model.InternalPhoneNumber = updatedAdministrator.InternalPhoneNumber;
            model.OfficeNumber = updatedAdministrator.OfficeNumber;
            model.Phone = updatedAdministrator.Phone;
            model.Email = updatedAdministrator.Email;
            model.Supervisor = updatedAdministrator.Supervisor;
            administrators[id] = model;
        }

        public IEnumerable<IUser> ListUsers() => users.Values;

        public object InsertUser(IUser newUser)
        {
            var newId = Guid.NewGuid();

            var dataModel = mapper.Map<UserDataModel>(newUser);
            dataModel.ID = newId;

            users.Add(newId, dataModel);
            return newId;
        }

        public void UpdateUser(IUser updatedUser)
        {
            Guid id = (Guid)updatedUser.ID;
            var model = mapper.Map<UserDataModel>(users[id]);
            model.InternalPhoneNumber = updatedUser.InternalPhoneNumber;
            model.OfficeNumber = updatedUser.OfficeNumber;
            model.Phone = updatedUser.Phone;
            model.Email = updatedUser.Email;
            model.Supervisor = updatedUser.Supervisor;
            users[id] = model;
        }

        public bool CheckUserId(object userId)
        {
            if (userId is Guid userGuid)
                return users.ContainsKey(userGuid);

            return false;
        }
    }
}