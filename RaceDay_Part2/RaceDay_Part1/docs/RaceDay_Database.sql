/*
 RaceDay - Part 1 Database Script
 SQL Server / SSMS
*/
IF DB_ID('RaceDayDB') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB;
END;
GO
CREATE DATABASE RaceDayDB;
GO
USE RaceDayDB;
GO

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Participant',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Organiser', 'Participant'))
);
GO

CREATE TABLE UserProfiles (
    UserProfileID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(30),
    Address NVARCHAR(250),
    EmergencyContactName NVARCHAR(100),
    EmergencyContactPhone NVARCHAR(30),
    CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500),
    EventDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL,
    EventType NVARCHAR(30) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Upcoming',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Events_Organiser FOREIGN KEY (OrganiserID) REFERENCES Users(UserID),
    CONSTRAINT CK_Events_Distance CHECK (DistanceKm > 0),
    CONSTRAINT CK_Events_Type CHECK (EventType IN ('Running','Walking','Cycling')),
    CONSTRAINT CK_Events_Status CHECK (Status IN ('Upcoming','Open','Completed','Cancelled'))
);
GO

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    AgeGroup NVARCHAR(50),
    EntryFee DECIMAL(10,2) NOT NULL DEFAULT 0,
    MaxParticipants INT,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE,
    CONSTRAINT UQ_Categories_Event_Name UNIQUE (EventID, CategoryName),
    CONSTRAINT CK_Categories_Fee CHECK (EntryFee >= 0),
    CONSTRAINT CK_Categories_Max CHECK (MaxParticipants IS NULL OR MaxParticipants > 0)
);
GO

CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    RaceNumber NVARCHAR(30),
    CONSTRAINT FK_Enrolments_Participant FOREIGN KEY (ParticipantID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enrolments_Category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT UQ_Enrolments_Participant_Category UNIQUE (ParticipantID, CategoryID),
    CONSTRAINT CK_Enrolments_Status CHECK (Status IN ('Pending','Confirmed','Cancelled'))
);
GO

CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishPosition INT,
    FinishTime TIME,
    ResultStatus NVARCHAR(20) NOT NULL DEFAULT 'Finished',
    RecordedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID),
    CONSTRAINT CK_Results_Position CHECK (FinishPosition IS NULL OR FinishPosition > 0),
    CONSTRAINT CK_Results_Status CHECK (ResultStatus IN ('Finished','DNF','DNS'))
);
GO

INSERT INTO Users (FirstName,LastName,Email,PasswordHash,Role) VALUES
('Thabo','Mokoena','thabo.organiser@raceday.co.za','DEMO_HASH_001','Organiser'),
('Lerato','Molefe','lerato.organiser@raceday.co.za','DEMO_HASH_002','Organiser'),
('Sipho','Dlamini','sipho.participant@raceday.co.za','DEMO_HASH_003','Participant'),
('Naledi','Mokoena','naledi.participant@raceday.co.za','DEMO_HASH_004','Participant');
GO

INSERT INTO UserProfiles (UserID,PhoneNumber,Address,EmergencyContactName,EmergencyContactPhone) VALUES
(1,'0711111111','Johannesburg, Gauteng','Kabelo Mokoena','0722222222'),
(2,'0733333333','Pretoria, Gauteng','Mpho Molefe','0744444444'),
(3,'0755555555','Soweto, Gauteng','Bongani Dlamini','0766666666'),
(4,'0777777777','Polokwane, Limpopo','Palesa Mokoena','0788888888');
GO

INSERT INTO Events (OrganiserID,EventName,Description,EventDate,StartTime,Location,DistanceKm,EventType,Status) VALUES
(1,'Polokwane Spring Run','Community road running event.','2026-10-10','07:00','Polokwane',10,'Running','Open'),
(1,'Soweto Community Walk','Community walking event.','2026-11-07','08:00','Soweto',8,'Walking','Upcoming'),
(2,'Limpopo Cycle Challenge','Road cycling event for local riders.','2026-12-05','06:30','Polokwane',50,'Cycling','Upcoming');
GO

INSERT INTO Categories (EventID,CategoryName,AgeGroup,EntryFee,MaxParticipants) VALUES
(1,'10km Open','18+',150,500),(1,'10km Junior','Under 18',80,200),
(2,'8km Open Walk','18+',100,400),(2,'8km Family Walk','All Ages',60,300),
(3,'50km Open','18+',250,250),(3,'50km Junior','Under 18',120,100);
GO

INSERT INTO Enrolments (ParticipantID,CategoryID,Status,RaceNumber) VALUES
(3,1,'Confirmed','PR001'),(3,3,'Confirmed','SW001'),
(4,2,'Confirmed','PR002'),(4,5,'Confirmed','LC001');
GO

INSERT INTO Results (EnrolmentID,FinishPosition,FinishTime,ResultStatus) VALUES
(1,12,'00:52:30','Finished'),(2,25,'01:04:10','Finished'),(3,8,'00:48:45','Finished');
GO

SELECT * FROM Users;
SELECT * FROM UserProfiles;
SELECT * FROM Events;
SELECT * FROM Categories;
SELECT * FROM Enrolments;
SELECT * FROM Results;
GO
