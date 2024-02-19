CREATE TABLE [dbo].[NationalAchievementRateOverallImport]
(
	[Id] BIGINT IDENTITY (1,1),
	[Age] INT NOT NULL DEFAULT 0,
	[ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	[SectorSubjectAreaTier1] INT NULL, 
    CONSTRAINT PK_NationalAchievementRateOverallImport PRIMARY KEY (Id)
)
GO
