-- DATABASE CREATION SECTION
use master
IF EXISTS(SELECT * FROM Sys.databases WHERE name = 'ST10091422_PROG6212_PART2')
DROP DATABASE ST10091422_PROG6212_PART2
CREATE DATABASE ST10091422_PROG6212_PART2
USE ST10091422_PROG6212_PART2

-- TABLE CREATION
CREATE TABLE [User](
	UserId INT not null IDENTITY(1,1),
	Username VARCHAR(100) NOT NULL,
	[Password] VARBINARY(32) NOT NULL,
	Salt VARBINARY(16) NOT NULL,
	tempPass VARCHAR(100) NOT NULL,
	CONSTRAINT User_pk PRIMARY KEY(UserId)
);

CREATE TABLE Semester(
	SemesterId INT NOT NULL IDENTITY(1,1) ,
	NumberOfWeeks INT,
	CurrentWeek INT,
	startDate Date,
	currentDate Date,
	weekStart Date,
	UserId INT,
	CONSTRAINT Semester_pk PRIMARY KEY(SemesterId),
	CONSTRAINT User_Semester_FK FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

CREATE TABLE Module(
	ModuleId INT not null IDENTITY(1,1),
	Code VARCHAR(50) not null,
	[Name] VARCHAR(50),
	NumberOfCredits INT,
	ClassHoursPerWeek INT,
	NumberOfSelfStudyHoursPerWeek INT,
	NumberOfHours INT,
	remainingHours INT,
	CurrentDate Date,
	SemesterId INT,
	CONSTRAINT Module_pk primary key (ModuleId),
	CONSTRAINT Semester_Module_fk FOREIGN KEY (SemesterId) REFERENCES Semester(SemesterId)
);

select * from Module;
select * from [User];
select * from Semester;

