CREATE TABLE [dbo].[NationalAchievementRateImport]
(
	[Id] BIGINT IDENTITY (1,1),
	[Ukprn] INT NOT NULL,
	[Age] SMALLINT NOT NULL DEFAULT 0,
	[SectorSubjectArea] VARCHAR(1000) NOT NULL,
	[ApprenticeshipLevel] SMALLINT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	CONSTRAINT PK_NationalAchievementRateImport PRIMARY KEY (Id)
)
GO
