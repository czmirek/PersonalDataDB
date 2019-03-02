namespace Tests
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using PersonalDataDB.Domain;

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var d = new PersonalDataDatabaseDomain("123", "cs-CZ");
            string json = JsonConvert.SerializeObject(d);
            
        }
    }
}