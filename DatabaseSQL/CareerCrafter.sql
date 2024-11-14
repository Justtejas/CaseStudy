create database CareerCrafter_DB;

CREATE TABLE dbo.[JobSeeker] (
    JobSeekerID INT NOT NULL PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    JobSeekerName NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    ContactPhone NVARCHAR(100) NOT NULL,
    DateOfBirth DATETIME NOT NULL,
    Gender NVARCHAR(MAX) NOT NULL,
    Address NVARCHAR(MAX) NOT NULL,
    Qualification NVARCHAR(MAX) NOT NULL,
    Specialization NVARCHAR(MAX) NOT NULL,
    Institute NVARCHAR(MAX) NOT NULL,
    Year INT NOT NULL,
    CGPA DECIMAL(4, 2) NOT NULL,
    CompanyName NVARCHAR(MAX) NOT NULL,
    Position NVARCHAR(MAX) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'JobSeeker',
    CONSTRAINT CHK_GENDER CHECK (Gender IN ('Male', 'Female')),
    CONSTRAINT CHK_Year CHECK (Year >= 1950 AND Year <= 2024),
    CONSTRAINT CHK_CGPA CHECK (CGPA >= 0.0 AND CGPA <= 10.0),
    CONSTRAINT UQ_JobSeeker_Username UNIQUE (Username),
    CONSTRAINT UQ_JobSeeker_Email UNIQUE (Email),
    CONSTRAINT UQ_JobSeeker_ContactPhone UNIQUE (ContactPhone)
);

CREATE TABLE dbo.[Resume] (
    ResumeID INT NOT NULL PRIMARY KEY,
    JobSeekerID INT NOT NULL,
	ResumeName NVARCHAR(MAX) NOT NULL,
	ResumeFileType NVARCHAR(MAX) NOT NULL,
	ResumeFileSize BIGINT NOT NULL,
	ResumeFileData VARBINARY(MAX) NOT NULL,
    UploadedDate DATETIME NOT NULL,
	ModifiedDate DATETIME NOT NULL,
    CONSTRAINT FK_Resume_JobSeeker FOREIGN KEY (JobSeekerID) REFERENCES JobSeeker(JobSeekerID) ON DELETE CASCADE ON UPDATE CASCADE
);



CREATE TABLE dbo.[Employer] (
    EmployerID INT NOT NULL PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL,
    Gender NVARCHAR(100) NOT NULL,
    EmployerName NVARCHAR(MAX) NOT NULL,
    CompanyName NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    ContactPhone NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'Employer',
    CONSTRAINT CHK_GENDERS CHECK (Gender IN ('Male', 'Female')),
    CONSTRAINT UQ_Employer_Username UNIQUE (Username),
    CONSTRAINT UQ_Employer_Email UNIQUE (Email),
    CONSTRAINT UQ_Employer_ContactPhone UNIQUE (ContactPhone)
);

CREATE TABLE dbo.[JobListing] (
    JobListingID INT NOT NULL PRIMARY KEY,
    EmployerID INT NOT NULL,
    JobTitle NVARCHAR(MAX) NOT NULL,
    JobDescription NVARCHAR(MAX) NOT NULL,
    CompanyName NVARCHAR(MAX) NOT NULL,
    HiringWorkflow NVARCHAR(MAX) NOT NULL,
    EligibilityCriteria NVARCHAR(MAX) NOT NULL,
    RequiredSkills NVARCHAR(MAX) NOT NULL,
    AboutCompany NVARCHAR(MAX) NOT NULL,
    Location NVARCHAR(MAX) NOT NULL,
    Salary DECIMAL(10, 2) NOT NULL,
    PostedDate DATETIME NOT NULL,
    Deadline DATETIME NOT NULL,
    VacancyOfJob BIT NOT NULL,
    CONSTRAINT CHK_Salary CHECK (Salary > 0),
    CONSTRAINT FK_JobListing_Employer FOREIGN KEY (EmployerID) REFERENCES Employer(EmployerID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE dbo.[Application] (
    ApplicationID INT NOT NULL PRIMARY KEY,
	JobSeekerID INT NOT NULL,
    JobListingID INT NOT NULL,
    ApplicationDate DATETIME NOT NULL,
    ApplicationStatus NVARCHAR(MAX) NOT NULL,
    CONSTRAINT CHK_Status CHECK (ApplicationStatus IN ('Confirmed', 'Pending', 'Cancelled')),
    CONSTRAINT FK_Application_JobListing FOREIGN KEY (JobListingID) REFERENCES JobListing(JobListingID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_Application_JobSeeker FOREIGN KEY (JobSeekerID) REFERENCES JobSeeker(JobSeekerID) ON DELETE CASCADE ON UPDATE CASCADE
);