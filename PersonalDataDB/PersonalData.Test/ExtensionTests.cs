namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using Xunit;
    public class ExtensionTests
    {
        
        [Fact]
        public void Create()
        {
            //TODO: normalni konstruktory
            InitializationDataBuilder b = new InitializationDataBuilder()
                .ConfigureDataManager(d =>
                {

                })
                .ConfigureAdministrator(a =>
                {
                    a.FullName = "";

                });
            
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(DataBuilderFake.GetFake());
        }       
    }
}