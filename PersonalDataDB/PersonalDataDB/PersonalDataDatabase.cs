using AutoMapper;
using PersonalDataDB.Data.Models;

namespace PersonalDataDB
{
    public class PersonalDataDatabase
    {
        private readonly static IMapper mapper;

        static PersonalDataDatabase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            mapper = config.CreateMapper();
        }

        public PersonalDataDatabase(IRepository repository)
        {
            Administrator = new PersonalDataAdministration(repository, mapper);
        }

        public PersonalDataAdministration Administrator { get; private set; }
        
    }
}
