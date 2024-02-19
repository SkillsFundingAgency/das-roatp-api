CREATE TABLE [dbo].[NationalAchievementRateOverall]
(
	[Id] BIGINT IDENTITY (1,1),
	[Age] INT NOT NULL DEFAULT 0,
	[ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	[SectorSubjectAreaTier1] INT NULL, 
    CONSTRAINT PK_NationalAchievementRateOverall PRIMARY KEY (Id)
)
GO

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRateOverall_SectorSubjectAreaTier1_Age_Level] ON [dbo].[NationalAchievementRateOverall] (SectorSubjectAreaTier1, Age, ApprenticeshipLevel) 
INCLUDE (Id, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 


CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRateOverall_SectorSubjectAreaTier1] ON [dbo].[NationalAchievementRateOverall] (SectorSubjectAreaTier1) 
INCLUDE (Id, OverallCohort, OverallAchievementRate, Age, ApprenticeshipLevel) WITH (ONLINE = ON) 
GO 