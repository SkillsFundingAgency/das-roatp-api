CREATE TABLE [dbo].[NationalAchievementRate]
(
	[Id] BIGINT IDENTITY (1,1),
	[ProviderId] INT NOT NULL,
	[Age] INT NOT NULL DEFAULT 0,
	[SectorSubjectArea] VARCHAR(1000) NOT NULL,
	[ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
	[OverallCohort] INT NULL,
	[OverallAchievementRate] decimal(10,4) NULL,
	CONSTRAINT PK_NationalAchievementRate PRIMARY KEY (Id),
	CONSTRAINT FK_NationalAchievementRate_Provider FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id)
)
GO

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_ProviderId] ON [dbo].[NationalAchievementRate] (ProviderId) 
INCLUDE (Id, [SectorSubjectArea], ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 

CREATE NONCLUSTERED INDEX [IDX_NationalAchievementRate_ProviderId_SectorSubjectArea] ON [dbo].[NationalAchievementRate] (ProviderId, SectorSubjectArea) 
INCLUDE (Id, ApprenticeshipLevel, OverallCohort, OverallAchievementRate) WITH (ONLINE = ON) 
GO 