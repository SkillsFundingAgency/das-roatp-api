CREATE TABLE [dbo].[NationalAchievementRate]
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

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_Ukprn] ON [dbo].[NationalAchievementRate] (Ukprn) 
INCLUDE (Id, [SectorSubjectArea], ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_Ukprn_SectorSubjectArea] ON [dbo].[NationalAchievementRate] (Ukprn, SectorSubjectArea) 
INCLUDE (Id, ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 