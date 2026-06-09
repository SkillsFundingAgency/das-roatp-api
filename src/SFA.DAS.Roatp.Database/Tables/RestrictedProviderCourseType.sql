CREATE TABLE [dbo].[RestrictedProviderCourseType]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Ukprn] BIGINT NOT NULL,
    [CourseType] NVARCHAR(50) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_RestrictedProviderCourseType PRIMARY KEY (Id),
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IXU_RestrictedProviderCourseType
ON [dbo].[RestrictedProviderCourseType] ([Ukprn],[CourseType]);
GO

