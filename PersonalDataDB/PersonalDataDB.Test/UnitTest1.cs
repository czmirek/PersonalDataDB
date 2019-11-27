using System;
using System.Collections.Generic;
using Xunit;

namespace PersonalDataDB.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            PersonalDataDBBuilder builder = new PersonalDataDBBuilder();
            PersonalDataDB pddb = builder
                   .UseDataProvider(new InMemoryDataProvider())
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
                   .Build();

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

            var readRowKvp = pddb.ReadRow("Persons", rowKey);
            var readRow = new Dictionary<string, object?>(readRowKvp);

            Assert.Equal(insertRow["Name"], readRow["Name"]);
            Assert.Equal(insertRow["Birthday"], readRow["Birthday"]);
            Assert.Equal(insertRow["NumberOfLimbs"], readRow["NumberOfLimbs"]);
            Assert.Equal(insertRow["HomeLatitude"], readRow["HomeLatitude"]);
            Assert.Equal(insertRow["BankAccount"], readRow["BankAccount"]);
            Assert.Equal(insertRow["IsGood"], readRow["IsGood"]);
            Assert.Equal(insertRow["DefaultPhoneID"], readRow["DefaultPhoneID"]);
        }
    }
}
