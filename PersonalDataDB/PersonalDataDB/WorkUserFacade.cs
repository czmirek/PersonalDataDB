namespace PersonalDataDB
{
    using System.Collections.Generic;

    public class WorkUserFacade
    {
        internal WorkUserFacade(System.Data.IDbConnection dbConnection) { }

        IPersonalDataValue ReadValue(int ownerID, string columnName) => null;
        IPersonalDataValue ReadValue(int ownerID, string columnName, int purposeID) => null;
        IPersonalDataValue ReadValue(int ownerID, string columnName, Purpose customPurpose) => null;

        IEnumerable<IPersonalDataValue> ReadValues(int ownerID) => null;
        IEnumerable<IPersonalDataValue> ReadValues(int ownerID, int purposeID) => null;
        IEnumerable<IPersonalDataValue> ReadValues(int ownerID, Purpose customPurpose) => null;

        T ReadValues<T>(int ownerID) where T : new() => default(T);
        T ReadValues<T>(int ownerID, int purposeID) where T : new() => default(T);
        T ReadValues<T>(int ownerID, Purpose purpose) where T : new() => default(T);

        IPersonalDataValue ReadTableValue(int ownerID, string tableName, int rowID) => null;
        IPersonalDataValue ReadTableValue(int ownerID, string tableName, int rowID, int purposeID) => null;
        IPersonalDataValue ReadTableValue(int ownerID, string tableName, int rowID, Purpose customPurpose) => null;

        IEnumerable<IPersonalDataValue> ReadTableValues(int ownerID, string tableName) => null;
        IEnumerable<IPersonalDataValue> ReadTableValues(int ownerID, string tableName, int purposeID) => null;
        IEnumerable<IPersonalDataValue> ReadTableValues(int ownerID, string tableName, Purpose customPurpose) => null;

        IEnumerable<T> ReadTableValues<T>(int ownerID) where T : new() => null;
        IEnumerable<T> ReadTableValues<T>(int ownerID, int purposeID) where T : new() => null;
        IEnumerable<T> ReadTableValues<T>(int ownerID, Purpose purpose) where T : new() => null;
    }
}