CREATE TABLE UserMaster (
    userId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    identifier VARCHAR(36),
    userName VARCHAR(50),
    Firstname VARCHAR(50),
    lastname VARCHAR(50),
    MiddleName VARCHAR(50),
    PhoneNo VARCHAR(12),
    emailId VARCHAR(50),
    HashPassword VARCHAR(50),
    RoleId INT,
    Jurisdiction INT,
    createdby INT,
    createdOn DATETIME,
    dob DATE,
    Aadhaar VARCHAR(12),
    pan VARCHAR(20),
    Status VARCHAR(10),
    Gender VARCHAR(10),
    Address VARCHAR(500),
    EmergencyContact VARCHAR(12),
    profilePhotoPath VARCHAR(500),
    DesignationId INT,
    ModifyBy INT,
    ModifyOn DATETIME
);

CREATE TABLE StateMaster (
    StateId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    StateName VARCHAR(50),
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME,
    CountryId INT
);

CREATE TABLE LocationMaster (
    LocationId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    LocationName VARCHAR(50),
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME,
    StateId INT
);

CREATE TABLE CrimeTypes (
    CrimeId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CrimeName VARCHAR(50),
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);

CREATE TABLE CountryMaster (
    CountryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CountryName VARCHAR(50),
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);

CREATE TABLE JurisdictionMaster (
    JurisdictionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    JurisdictionName VARCHAR(50),
    Identifier VARCHAR(36),
    LocationId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);

CREATE TABLE EvidenceTypes (
    EvidenceTypeId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EvidenceName VARCHAR(50),
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE DesignationMaster (
    DesignationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    DesignationName VARCHAR(50),
    Identifier VARCHAR(36),
    RoleId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE CaseNotes (
    CaseNoteId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Identifier VARCHAR(36),
    CaseNote VARCHAR(5000),
    CrimeId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);

CREATE TABLE Criminals (
    CriminalId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CriminalName VARCHAR(50),
    CrimeTypeId INT,
    JurisdictionId INT,
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    userId INT,
    Identifier VARCHAR(36),
    Title VARCHAR(50),
    Message VARCHAR(200),
    IsRead BIT,
    type VARCHAR(50),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    RoleName VARCHAR(50),
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE EvidenceAttachment (
    EvidenceAttachmentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EvidenceAttachmentPath VARCHAR(100),
    Identifier VARCHAR(36),
    ComplaintId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE CrimeCriminals (
    CrimeCriminalId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CrimeId INT,
    CriminalId INT,
    Identifier VARCHAR(36),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME,
    EvidenceAttachment VARCHAR(100)
);


CREATE TABLE FIRAttachment (
    FIRAttachmentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AttachmentPath VARCHAR(100),
    ComplaintId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE ComplaintIOAssign (
    AssignId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ComplaintId INT,
    Identifier VARCHAR(36),
    userId INT,
    ComplaintStatus INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE EvidenceAttachmentBackup (
    EvidenceAttachmentBackupId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EvidenceAttachmentPath VARCHAR(100),
    Identifier VARCHAR(36),
    ComplaintId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE Investigations (
    InvestigationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Identifier VARCHAR(36),
    ComplaintId INT,
    IoOfficerId INT,
    StartDate DATETIME,
    EndDate DATETIME,
    Priority VARCHAR(20),
    StatusId INT,
    EvidenceAttachmentId INT,
    CrimeId INT,
    CriminalId INT,
    FIRAttachmentId INT,
    InvestigationDescription VARCHAR(1000),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE InvestigationStageHistory (
    StageId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Identifier VARCHAR(36),
    InvestigationStageHistory VARCHAR(500),
    StartDate DATETIME,
    EndDate DATETIME,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE ComplaintRequest (
    ComplaintRequestId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    identifier VARCHAR(36),
    ComplaintName VARCHAR(100),
    JurisdictionID INT,
    CrimeTypeId INT,
    CrimeDescription VARCHAR(1000),
    PhoneNumber VARCHAR(12),
    EvidenceAttachmentId INT,
    IOofficerId INT,
    InvestigationId INT,
    StatusId INT,
    DateReported DATETIME,
    Priority VARCHAR(20),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);


CREATE TABLE ComplaintRequestBackup (
    ComplaintRequestBackUpId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    identifier VARCHAR(36),
    ComplaintName VARCHAR(100),
    JurisdictionID INT,
    CrimeTypeId INT,
    CrimeDescription VARCHAR(1000),
    PhoneNumber VARCHAR(12),
    EvidenceAttachmentId INT,
    IOofficerId INT,
    InvestigationId INT,
    StatusId INT,
    DateReported DATETIME,
    Priority VARCHAR(20),
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);

CREATE TABLE UserLoginLog (
    UserLoginId INT IDENTITY(1,1) PRIMARY KEY,
    userId INT,
    loginDateTime DATETIME
);

CREATE TABLE IOJurisdictionAssign (
    IOJurisdictionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Identifier VARCHAR(36),
    userId INT,
    JurisdictionId INT,
    CreatedBy INT,
    CreatedOn DATETIME,
    ModifyBy INT,
    ModifyOn DATETIME
);



