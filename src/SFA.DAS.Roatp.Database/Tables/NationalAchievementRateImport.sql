﻿CREATE TABLE [dbo].[NationalAchievementRateImport]
(
    [Id] BIGINT IDENTITY (1,1),
    [Ukprn] INT NOT NULL,
    [Age] INT NOT NULL DEFAULT 0,
    [ApprenticeshipLevel] INT NOT NULL DEFAULT 0,
    [OverallCohort] INT NULL,
    [OverallAchievementRate] decimal(10,4) NULL,
    [SectorSubjectAreaTier1] INT NOT NULL DEFAULT 0, 
    CONSTRAINT PK_NationalAchievementRateImport PRIMARY KEY (Id)
)
GO
