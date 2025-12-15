CREATE TABLE [dbo].[Standard]
(
    [StandardUId] VARCHAR(20) NOT NULL,
    [LarsCode] NVARCHAR(10) NOT NULL,
    [IfateReferenceNumber] VARCHAR(10) NOT NULL,
    [Level] INT NOT NULL,
    [Title] VARCHAR(1000) NOT NULL,
    [Version] VARCHAR(10) NOT NULL,
    [ApprovalBody] VARCHAR(1000) NULL, 
    [SectorSubjectArea] VARCHAR(1000) NULL, 
    [SectorSubjectAreaTier1] INT NULL, 
    [IsRegulatedForProvider] BIT NOT NULL DEFAULT 0,
    [Duration] INT NULL,
    [DurationUnits] VARCHAR(6) NULL,
    [Route] VARCHAR(500) NULL,
    [ApprenticeshipType] Varchar(50) NULL,
    [CourseType] NVarchar(50) NULL,
    CONSTRAINT PK_Standard PRIMARY KEY (StandardUId)
)
