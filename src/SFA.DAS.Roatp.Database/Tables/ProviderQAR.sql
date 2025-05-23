﻿CREATE TABLE [dbo].[ProviderQAR]
(
  [TimePeriod] VARCHAR(4) NOT NULL,
  [Ukprn] BIGINT  NOT NULL,
  [Leavers] VARCHAR(10) NOT NULL,
  [AchievementRate] VARCHAR(10) NOT NULL,
  [CreatedDate] DATETIME2  NOT NULL DEFAULT GETUTCDATE(),
  [AchievementRank] AS (CONVERT(VARCHAR(20)
                       ,CASE WHEN ISNULL([AchievementRate],'x') LIKE N'%[^0-9.]%'
                             THEN 'None'
                             WHEN [AchievementRate]='100' then 'Excellent' 
                             WHEN [AchievementRate] < '50' THEN 'VeryPoor'
                             WHEN [AchievementRate] < '60' THEN 'Poor'
                             WHEN [AchievementRate] < '70' THEN 'Good'
                             ELSE 'Excellent' END))
);
GO

CREATE UNIQUE INDEX IXU_ProviderQAR_TimePeriod ON [dbo].[ProviderQAR]
( [TimePeriod], [Ukprn] ) INCLUDE ( [Leavers], [AchievementRate], [AchievementRank] );
GO
