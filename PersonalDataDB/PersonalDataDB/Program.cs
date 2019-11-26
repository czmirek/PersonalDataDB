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
        }
    }
}
