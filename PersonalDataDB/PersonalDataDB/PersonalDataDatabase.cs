namespace PersonalDataDB
{
    using Dapper;
    using System.Data;
    using Dapper.Contrib.Extensions;
    using System.Data.SQLite;

    public class PersonalDataDatabase
    {
        public PersonalDataDatabase(IDbConnection db)
        {
            this.db = db;
            this.sysLogger = new SysLogger(db, new DefaultDateTimeProvider());
            Administrator = new PersonalDataAdministration(db, sysLogger);
            WorkUser = new WorkUserFacade(db);
            DataOwner = new PersonalDataManipulation(db);
        }

        private readonly IDbConnection db;
        private readonly SysLogger sysLogger;

        public PersonalDataAdministration Administrator { get; private set; }
        public WorkUserFacade WorkUser { get; private set; }
        public PersonalDataManipulation DataOwner { get; private set; }

        public IDateTimeProvider DateTimeProvider { get; private set; }
        public Configuration Configuration { get; private set; }

        public static PersonalDataDatabase OpenSqlite(string sqliteFile)
        {
            return new PersonalDataDatabase(new SQLiteConnection($"Data Source={sqliteFile}"));
        }

        public static PersonalDataDatabase CreateSqlite(string sqliteFile, Configuration databaseConfiguration)
        {
            using (var db = new SQLiteConnection($"Data Source={sqliteFile}"))
            {
                var pddb = new PersonalDataDatabase(db);
                pddb.Initialize();

                return pddb;
            }
        }

        private void Initialize()
        {
            db.Execute(@"
DROP TABLE IF EXISTS Agreement;
CREATE TABLE Agreement(
    ID INT PRIMARY KEY,
    OwnerID INT NULL REFERENCES Owner(ID),
    RowID INT NULL,
    'Table' TEXT,
    'Column' TEXT
);
DROP TABLE IF EXISTS AgreementProof;
CREATE TABLE AgreementProof(
    ID INT PRIMARY KEY,
    AgreementID INT NULL REFERENCES Agreement(ID),
    Created DATETIME,
    Proof TEXT
);
DROP TABLE IF EXISTS AgreementText;
CREATE TABLE AgreementText(
    ID INT PRIMARY KEY,
    AgreementID INT REFERENCES Agreement(ID),
    LanguageID INT REFERENCES Language(ID),
    'Order' INT,
    Text TEXT
);
DROP TABLE IF EXISTS Configuration;
CREATE TABLE Configuration(
    'Key' TEXT PRIMARY KEY,
    'Value' TEXT
);
DROP TABLE IF EXISTS Language;
CREATE TABLE Language(
    ID INT PRIMARY KEY,
    LanguageCode TEXT
);
DROP TABLE IF EXISTS LimitingReason;
CREATE TABLE LimitingReason(
    ID INT PRIMARY KEY,
    Type INT
);
DROP TABLE IF EXISTS LimitingReasonText;
CREATE TABLE LimitingReasonText(
    ID INT PRIMARY KEY,
    LimitingReasonID INT REFERENCES LimitingReason(ID),
    LanguageID INT REFERENCES Language(ID),
    'Order' INT,
    Text TEXT
);
DROP TABLE IF EXISTS Owner;
CREATE TABLE Owner(
    ID INT PRIMARY KEY,
    ExternalID INT NULL
);
DROP TABLE IF EXISTS PersonalDataLog;
CREATE TABLE PersonalDataLog(
    ID INT PRIMARY KEY,
    WorkUserID INT REFERENCES WorkUser(ID),
    OwnerID INT REFERENCES Owner(ID),
    RowID INT NULL,
    'Table' TEXT,
    'Column' TEXT,
    Created DATETIME,
    Log TEXT
);

DROP TABLE IF EXISTS PersonalDataReference;
CREATE TABLE PersonalDataReference(
    ID INT PRIMARY KEY,
    Created DATETIME,
    OwnerID INT NULL REFERENCES Owner(ID),
    RowID INT NULL,
    'Table' TEXT,
    'Column' TEXT,
    Location TEXT,
    Reference TEXT,
    Url TEXT,
    File TEXT,
    Email TEXT,
    Print TEXT,
    Letter TEXT
);

DROP TABLE IF EXISTS Purpose;
CREATE TABLE Purpose(
    ID INT PRIMARY KEY,
    Type INT,
    OwnerID INT NULL REFERENCES Owner(ID),
    RowID INT NULL,
    'Table' TEXT,
    'Column' TEXT
);

DROP TABLE IF EXISTS PurposeQuestion;
CREATE TABLE PurposeQuestion(
    ID INT PRIMARY KEY,
    PurposeID INT REFERENCES Purpose(ID),
    LanguageID INT REFERENCES Language(ID),
    DefaultPurposeAnswerID INT REFERENCES PurposeAnswer(ID),
    'Order' INT,
    QuestionFromOwnerPerspective TEXT
);

DROP TABLE IF EXISTS PurposeAnswer;
CREATE TABLE PurposeAnswer(
    ID INT PRIMARY KEY,
    PurposeQuestionID INT REFERENCES PurposeQuestion(ID),
    Answer TEXT
);

DROP TABLE IF EXISTS SystemLog;
CREATE TABLE SystemLog(
    ID INT PRIMARY KEY,
    Time DATETIME,
    Log TEXT
);

DROP TABLE IF EXISTS WorkUser;
CREATE TABLE WorkUser(
    ID INT PRIMARY KEY,
    ExternalID INT NULL,
    Active BOOL,
    FullName TEXT,
    Created DATETIME
);

DROP TABLE IF EXISTS WorkUserText;
CREATE TABLE WorkUserText(
    ID INT PRIMARY KEY,
    WorkUserID INT REFERENCES WorkUser(ID),
    LanguageID INT REFERENCES Language(ID),
    Key TEXT,
    Text TEXT
);");
        }
    }
}
