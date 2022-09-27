CREATE TABLE [dbo].[NationalAchievementRate_Import]
(
	[Id] BIGINT PRIMARY KEY IDENTITY (1,1),
	[Ukprn] INT NOT NULL,
	[Age] SMALLINT NOT NULL DEFAULT 0,
	[SectorSubjectArea] VARCHAR(1000) NOT NULL,
	[ApprenticeshipLevel] SMALLINT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL
)
GO
