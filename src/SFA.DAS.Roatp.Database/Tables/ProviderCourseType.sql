CREATE TABLE [dbo].[ProviderCourseType]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Ukprn] INT NOT NULL,
	[CourseType] nvarchar(50) NOT NULL,
	[LearningType] nvarchar(20) NOT NULL
);
GO;

CREATE NONCLUSTERED INDEX IX_ProviderCourseType_Ukprn_CourseType_LearningType
ON [dbo].[ProviderCourseType] ([Ukprn],[CourseType],[LearningType])
INCLUDE ([Id]);