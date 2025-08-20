-- Users table for authentication
CREATE TABLE Users (
    UserID int IDENTITY(1,1) PRIMARY KEY,
    Email nvarchar(255) UNIQUE NOT NULL,
    PasswordHash nvarchar(255) NOT NULL,
    PasswordSalt nvarchar(255) NOT NULL,
    Role nvarchar(50) NOT NULL CHECK (Role IN ('Admin', 'Doctor', 'Receptionist')),
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    IsActive bit DEFAULT 1,
    CreatedDate datetime2 DEFAULT GETDATE(),
    ModifiedDate datetime2 DEFAULT GETDATE()
);

-- Patients table
CREATE TABLE Patients (
    PatientID int IDENTITY(1,1) PRIMARY KEY,
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    DateOfBirth date NOT NULL,
    Gender nvarchar(10) CHECK (Gender IN ('Male', 'Female', 'Other')),
    Email nvarchar(255),
    Phone nvarchar(20),
    Address nvarchar(500),
    EmergencyContact nvarchar(255),
    EmergencyPhone nvarchar(20),
    IsActive bit DEFAULT 1,
    CreatedDate datetime2 DEFAULT GETDATE(),
    ModifiedDate datetime2 DEFAULT GETDATE()
);

-- Doctors table
CREATE TABLE Doctors (
    DoctorID int IDENTITY(1,1) PRIMARY KEY,
    UserID int,
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    Specialization nvarchar(100),
    LicenseNumber nvarchar(50) UNIQUE,
    Email nvarchar(255),
    Phone nvarchar(20),
    IsActive bit DEFAULT 1,
    CreatedDate datetime2 DEFAULT GETDATE(),
    ModifiedDate datetime2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Fee Schedule table
CREATE TABLE FeeSchedule (
    FeeScheduleID int IDENTITY(1,1) PRIMARY KEY,
    ServiceName nvarchar(200) NOT NULL,
    ServiceCode nvarchar(50) UNIQUE,
    Amount decimal(10,2) NOT NULL,
    Description nvarchar(500),
    IsActive bit DEFAULT 1,
    CreatedDate datetime2 DEFAULT GETDATE(),
    ModifiedDate datetime2 DEFAULT GETDATE()
);

-- Visits table
CREATE TABLE Visits (
    VisitID int IDENTITY(1,1) PRIMARY KEY,
    PatientID int NOT NULL,
    DoctorID int NOT NULL,
    VisitDate datetime2 NOT NULL,
    VisitType nvarchar(50) DEFAULT 'Regular',
    Status nvarchar(20) DEFAULT 'Scheduled' CHECK (Status IN ('Scheduled', 'InProgress', 'Completed', 'Cancelled')),
    Reason nvarchar(500),
    Notes nvarchar(1000),
    Duration int DEFAULT 30, -- in minutes
    TotalAmount decimal(10,2) DEFAULT 0,
    CreatedDate datetime2 DEFAULT GETDATE(),
    ModifiedDate datetime2 DEFAULT GETDATE(),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);

-- Visit Services table (many-to-many relationship between visits and fee schedule)
CREATE TABLE VisitServices (
    VisitServiceID int IDENTITY(1,1) PRIMARY KEY,
    VisitID int NOT NULL,
    FeeScheduleID int NOT NULL,
    Quantity int DEFAULT 1,
    Amount decimal(10,2) NOT NULL,
    CreatedDate datetime2 DEFAULT GETDATE(),
    FOREIGN KEY (VisitID) REFERENCES Visits(VisitID),
    FOREIGN KEY (FeeScheduleID) REFERENCES FeeSchedule(FeeScheduleID)
);

-- Activity Log table
CREATE TABLE ActivityLogs (
    ActivityLogID int IDENTITY(1,1) PRIMARY KEY,
    UserID int,
    EntityType nvarchar(50) NOT NULL, -- Patient, Doctor, Visit, etc.
    EntityID int NOT NULL,
    Action nvarchar(50) NOT NULL, -- Created, Updated, Deleted, etc.
    Details nvarchar(1000),
    IPAddress nvarchar(45),
    CreatedDate datetime2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Indexes for better performance
CREATE INDEX IX_Patients_LastName ON Patients(LastName);
CREATE INDEX IX_Patients_Email ON Patients(Email);
CREATE INDEX IX_Doctors_Specialization ON Doctors(Specialization);
CREATE INDEX IX_Visits_VisitDate ON Visits(VisitDate);
CREATE INDEX IX_Visits_PatientID ON Visits(PatientID);
CREATE INDEX IX_Visits_DoctorID ON Visits(DoctorID);
CREATE INDEX IX_ActivityLogs_EntityType_EntityID ON ActivityLogs(EntityType, EntityID);
CREATE INDEX IX_Users_Email ON Users(Email);
