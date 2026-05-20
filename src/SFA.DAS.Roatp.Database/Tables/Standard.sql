CREATE TABLE [dbo].[Standard]
(
    [StandardUId] VARCHAR(20) NOT NULL,
    [LarsCode] NVARCHAR(10) NOT NULL,
    [IfateReferenceNumber] VARCHAR(10) NOT NULL,
    [Level] INT NOT NULL,
    [Title] VARCHAR(1000) NOT NULL,
    [ApprovalBody] VARCHAR(1000) NULL,
    [IsRegulatedForProvider] BIT NOT NULL DEFAULT 0,
    [Duration] INT NOT NULL,
    [DurationUnits] VARCHAR(6) NOT NULL,
    [Route] VARCHAR(500) NOT NULL,
    [ApprenticeshipType] Varchar(50) NOT NULL,
    [CourseType] NVarchar(50) NOT NULL,
    [IsActiveAvailable] BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Standard PRIMARY KEY (StandardUId)
)
