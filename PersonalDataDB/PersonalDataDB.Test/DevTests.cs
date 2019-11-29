using System;
using System.Collections.Generic;
using Xunit;

namespace PersonalDataDB.Test
{
    public class DevTests
    {
        private PersonalDataDB pddb;
        public DevTests()
        {
            PersonalDataDBBuilder builder = new PersonalDataDBBuilder();
            this.pddb = builder
                   .UseDataProvider(tableSet => new InMemoryDataProvider(tableSet))
                   .ConfigureTables(tables =>
                   {
                       tables.Add("Persons", columns =>
                       {
                           columns.AddScalar("Name", ColumnType.String, isNullable: false);
                           columns.AddScalar("Birthday", ColumnType.DateTime, isNullable: false);
                           columns.AddScalar("NumberOfLimbs", ColumnType.Integer, isNullable: false);
                           columns.AddScalar("HomeLatitude", ColumnType.Double, isNullable: false);
                           columns.AddScalar("BankAccount", ColumnType.Decimal, isNullable: false);
                           columns.AddScalar("IsGood", ColumnType.Bool, isNullable: false);
                           columns.AddForeignKey("DefaultPhoneID", "PhoneNumbers", isNullable: true);
                       });

                       tables.Add("PhoneNumbers", columns =>
                       {
                           columns.AddScalar("Number", ColumnType.String, isNullable: false);
                           columns.AddForeignKey("PersonID", "Persons", isNullable: false);
                       });
                   })
                   /*
                   .CreateDatabase()
                   .CreateDatabaseIfNotExist()
                   .UpdateDatabase()*/
                   .Build();

        }
        
        [Fact]
        public void InsertsAndReads()
        {
            var insertRow = new Dictionary<string, object?>()
            {
                { "Name", "Karel" },
                { "Birthday", DateTime.Now.AddYears(-30) },
                { "NumberOfLimbs", 5 },
                { "HomeLatitude", 2.46876846 },
                { "BankAccount", 1005.50M },
                { "IsGood", true },
                { "DefaultPhoneID", null }
            };
            
            object rowKey = pddb.Insert("Persons", insertRow);

            Assert.NotNull(rowKey);

            var readRow = new Dictionary<string, object?>(pddb.ReadRow("Persons", rowKey));

            pddb.Update("Persons", rowKey, "IsGood", false);
            pddb.Update("Persons", rowKey, new Dictionary<string, object?>()
            {
                { "Name", "Karel Varel" },
                { "HomeLatitude", 3.798846}
            });

            var readRow2 = new Dictionary<string, object?>(pddb.ReadRow("Persons", rowKey));

            Assert.Equal(insertRow["Name"], readRow["Name"]);
            Assert.Equal("Karel Varel", readRow2["Name"]);

            Assert.Equal(insertRow["Birthday"], readRow["Birthday"]);
            Assert.Equal(insertRow["NumberOfLimbs"], readRow["NumberOfLimbs"]);
            
            Assert.Equal(insertRow["HomeLatitude"], readRow["HomeLatitude"]);
            Assert.Equal(3.798846, readRow2["HomeLatitude"]);

            Assert.Equal(insertRow["BankAccount"], readRow["BankAccount"]);
            
            Assert.Equal(insertRow["IsGood"], readRow["IsGood"]);
            Assert.Equal(false, readRow2["IsGood"]);

            Assert.Equal(insertRow["DefaultPhoneID"], readRow["DefaultPhoneID"]);

            var readCols3 = pddb.ReadRow("Persons", rowKey, new string[] { "Birthday", "NumberOfLimbs" });

            Assert.Equal(insertRow["Birthday"], readCols3["Birthday"]);
            Assert.Equal(insertRow["NumberOfLimbs"], readCols3["NumberOfLimbs"]);
        }

        [Fact]
        public void ForeignKeys()
        {
            var person = new Dictionary<string, object?>()
            {
                { "Name", "Karel" },
                { "Birthday", DateTime.Now.AddYears(-30) },
                { "NumberOfLimbs", 5 },
                { "HomeLatitude", 2.46876846 },
                { "BankAccount", 1005.50M },
                { "IsGood", true },
                { "DefaultPhoneID", null }
            };

            object rowKey = pddb.Insert("Persons", person);

            var phone = new Dictionary<string, object?>()
            {
                {"Number", "123456789"},
                {"PersonID", rowKey }
            };

            pddb.Insert("PhoneNumbers", phone);
        }
    }
}