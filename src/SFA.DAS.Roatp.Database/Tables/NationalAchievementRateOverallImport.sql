CREATE TABLE [dbo].[NationalAchievementRateOverallImport]
(
	[Id] BIGINT IDENTITY (1,1),
	[Age] INT NOT NULL DEFAULT 0,
	[SectorSubjectArea] VARCHAR(1000) NOT NULL,
	[ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	CONSTRAINT PK_NationalAchievementRateOverallImport PRIMARY KEY (Id)
)
GO
