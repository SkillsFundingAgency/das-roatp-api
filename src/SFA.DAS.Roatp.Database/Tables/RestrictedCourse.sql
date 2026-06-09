CREATE TABLE [dbo].[RestrictedCourse]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [LarsCode] NVARCHAR(10) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_RestrictedCourse PRIMARY KEY (Id)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IXU_RestrictedCourse_LarsCode
ON [dbo].[RestrictedCourse] ([LarsCode]) INCLUDE ([Id],[CreatedDate]);
GO

