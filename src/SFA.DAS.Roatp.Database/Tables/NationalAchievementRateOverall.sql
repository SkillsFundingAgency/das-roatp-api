CREATE TABLE [dbo].[NationalAchievementRateOverall]
(
	[Id] BIGINT IDENTITY (1,1),
	[Age] SMALLINT NOT NULL DEFAULT 0,
	[SectorSubjectArea] VARCHAR(1000) NOT NULL,
	[ApprenticeshipLevel] SMALLINT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	CONSTRAINT PK_NationalAchievementRateOverall PRIMARY KEY (Id)
)
GO

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRateOverall_Sector] ON [dbo].[NationalAchievementRateOverall] (SectorSubjectArea, Age, ApprenticeshipLevel) 
INCLUDE (Id, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 


CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRateOverall_Sectordescription] ON [dbo].[NationalAchievementRateOverall] (SectorSubjectArea) 
INCLUDE (Id, OverallCohort, OverallAchievementRate, Age, ApprenticeshipLevel) WITH (ONLINE = ON) 
GO 