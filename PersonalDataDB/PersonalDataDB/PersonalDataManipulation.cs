using System.Collections.Generic;

namespace PersonalDataDB
{
    public class PersonalDataManipulation
    {
        internal PersonalDataManipulation(System.Data.IDbConnection dbConnection) { }

        // čtu vlastní data jako vlastník
        // Data se normálně vytáhnou z databáze
        // každý osobní údaj se zaloguje ve formátu "Já jako vlastník jsem si vytáhnul tyhle osobní údaje: A,B..."
        // tohle logování jde vypnout
        // nepotřebuji žádný účel ani důkaz

        // chci si jako vlastník přečíst všechna svoje data
        IEnumerable<IPersonalDataValue> ReadAllMyPersonalData(int ownerID) => null;
        void ReadAllMyPersonalData<T>(int ownerID) where T : new() { }

        // chci si jako vlastník přečíst konkrétní osobní údaj ve vztahu 1:1
        IPersonalDataValue ReadMyPersonalData(int ownerID, string personalDataID) => null;

        // chci si jako vlastník přečíst konkrétní osobní údaje ve vztahu 1:n
        IPersonalDataValue ReadMyPersonalData(int ownerID, string personalDataID, int rowID) => null;

        // chci si jako vlastník aktualizovat jeden osobní údaj
        // tohle udělá log + zároveň to lze omezit existujícím důvodem, že to nejde to pak musí vyhodit exceptionu

        // 1:1
        void SetMyPersonalData(int ownerID, string personalDataID, object value) { }

        // 1:n

        void SetMyPersonalData(int ownerID, string personalDataID, int rowID, object value) { }

        // chci si jako vlastník zrušit svůj osobní údaj
        // stejně jako update: lze omezit důvodem co vyhodí exceptionu

        //1:1
        void UnsetMyPersonalData(int ownerID, string personalDataID) { }

        //1:n
        void UnsetMyPersonalData(int ownerID, string personalDataID, int rowID) { }

        // chci jako vlastník smazat všechny své osobní údaje
        // stejně jako výše: lze omezit důvodem co vyhodí exceptionu
        void PurgeAllMyPersonalData(int ownerID) { }

        // chci jako vlastník přečíst všechny své osobní údaje
        // lze vždy, ve vrstvě to nedělá vůbec nic
        IEnumerable<PersonalDataAccessLog> ReadLogsOnMyPersonalData() => null;
    }
}