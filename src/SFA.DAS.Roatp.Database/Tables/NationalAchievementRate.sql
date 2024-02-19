CREATE TABLE [dbo].[NationalAchievementRate]
(
    [Id] BIGINT IDENTITY (1,1),
    [Age] INT NOT NULL DEFAULT 0,
    [ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
    [OverallCohort] INT NULL,
    [OverallAchievementRate] decimal(10,4) NULL,
    [SectorSubjectAreaTier1] INT NOT NULL DEFAULT 0, 
    [Ukprn] INT NOT NULL DEFAULT 0, 
    CONSTRAINT PK_NationalAchievementRate PRIMARY KEY (Id),
)
GO

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_Ukprn] ON [dbo].[NationalAchievementRate] (Ukprn) 
INCLUDE (Id, [SectorSubjectAreaTier1], ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_Ukprn_SectorSubjectAreaTier1] ON [dbo].[NationalAchievementRate] (Ukprn, SectorSubjectAreaTier1) 
INCLUDE (Id, ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 
