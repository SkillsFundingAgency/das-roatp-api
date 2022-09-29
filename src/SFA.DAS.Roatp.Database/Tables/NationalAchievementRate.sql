CREATE TABLE [dbo].[NationalAchievementRate]
(
	[Id] BIGINT IDENTITY (1,1),
	[Ukprn] INT NOT NULL,
	[Age] INT NOT NULL DEFAULT 0,
	[SectorSubjectArea] VARCHAR(1000) NOT NULL,
	[ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	CONSTRAINT PK_NationalAchievementRate PRIMARY KEY (Id)
)
GO

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_Ukprn] ON [dbo].[NationalAchievementRate] (Ukprn) 
INCLUDE (Id, [SectorSubjectArea], ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_Ukprn_SectorSubjectArea] ON [dbo].[NationalAchievementRate] (Ukprn, SectorSubjectArea) 
INCLUDE (Id, ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 