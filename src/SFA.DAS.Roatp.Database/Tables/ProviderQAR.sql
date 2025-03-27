CREATE TABLE [dbo].[ProviderQAR]
(
  [TimePeriod] VARCHAR(4) NOT NULL,
  [Ukprn] BIGINT  NOT NULL,
  [Leavers] VARCHAR(10) NOT NULL,
  [AchievementRate] VARCHAR(10) NOT NULL,
  [CreatedDate] DATETIME2  NOT NULL DEFAULT GETUTCDATE(),
  [AchievementRank] AS (CONVERT(VARCHAR(20)
                       ,CASE WHEN ISNULL([AchievementRate],'x') = 'x' 
                             THEN 'None'
                             WHEN [AchievementRate]='100' then 'Excellent' 
                             WHEN [AchievementRate] < '50' THEN 'VeryPoor'
                             WHEN [AchievementRate] < '60' THEN 'Poor'
                             WHEN [AchievementRate] < '70' THEN 'Good'
                             ELSE 'Excellent' END)),   
  CONSTRAINT PK_ProviderQAR PRIMARY KEY ([TimePeriod], [Ukprn])
);
GO

