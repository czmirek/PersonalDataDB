using System;
using System.Collections.Generic;

namespace PersonalDataDB
{
    class Program
    {
        static void Main(string[] args)
        {
            PersonalDataDBBuilder builder = new PersonalDataDBBuilder();
            PersonalDataDB pddb = builder
                   .UseDataProvider(dataSet => new InMemoryDataProvider(dataSet))
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

            pddb.Insert("Persons", new Dictionary<string, object?>()
            {
                { "Name", "Karel" },
                { "Birthday", DateTime.Now.AddYears(-30) },
                { "NumberOfLimbs", 5 },
                { "HomeLatitude", 2.46876846 },
                { "BankAccount", 1005.50M },
                { "IsGood", true },
                { "DefaultPhoneID", null }
            });
        }
    }
}
