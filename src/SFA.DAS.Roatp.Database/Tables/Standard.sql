CREATE TABLE [dbo].[Standard]
(
    [StandardUId] VARCHAR(20) NOT NULL,
    [LarsCode] INT NOT NULL,
    [IfateReferenceNumber] VARCHAR(10) NOT NULL,
    [Level] INT NOT NULL,
    [Title] VARCHAR(1000) NOT NULL,
    [Version] VARCHAR(10) NOT NULL,
    CONSTRAINT PK_Course PRIMARY KEY (StandardUId)
)
