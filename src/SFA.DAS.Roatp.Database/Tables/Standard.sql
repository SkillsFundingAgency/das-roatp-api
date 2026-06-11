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
    [LearningType] Varchar(50) NOT NULL,
    [CourseType] NVarchar(50) NOT NULL,
    [IsActiveAvailable] BIT NOT NULL DEFAULT 0,
    [LastDateStarts] DATETIME NULL,
    CONSTRAINT PK_Standard PRIMARY KEY (StandardUId)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IXU_Standard_LarsCode
ON [dbo].[Standard] ([LarsCode]) INCLUDE ([LearningType],[CourseType],[IfateReferenceNumber],[Title],[Level],[LastDateStarts]);
GO
