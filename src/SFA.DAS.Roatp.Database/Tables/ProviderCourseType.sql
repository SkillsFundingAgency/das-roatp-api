CREATE TABLE [dbo].[ProviderCourseType]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Ukprn] INT NOT NULL,
	[CourseType] nvarchar(50) NOT NULL,
);
GO;

CREATE NONCLUSTERED INDEX IX_ProviderCourseType_Ukprn_CourseType
ON [dbo].[ProviderCourseType] ([Ukprn],[CourseType])
INCLUDE ([Id]);