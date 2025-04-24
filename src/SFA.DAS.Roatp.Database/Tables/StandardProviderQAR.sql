CREATE TABLE [dbo].[StandardProviderQAR]
(
  [TimePeriod] VARCHAR(4), 
  [IfateReferenceNumber] VARCHAR(10),
  [Ukprn] bigint, 
  [Leavers] VARCHAR(10), 
  [AchievementRate] VARCHAR(10),
  [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
  [AchievementRank] AS (CONVERT(VARCHAR(20)
                       ,CASE WHEN ISNULL([AchievementRate],'x') LIKE N'%[^0-9.]%' 
                             THEN 'None'
                             WHEN [AchievementRate]='100' then 'Excellent' 
                             WHEN [AchievementRate] < '50' THEN 'VeryPoor'
                             WHEN [AchievementRate] < '60' THEN 'Poor'
                             WHEN [AchievementRate] < '70' THEN 'Good'
                             ELSE 'Excellent' END)),    
  --CONSTRAINT PK_StandardProviderQAR PRIMARY KEY ([TimePeriod], [Ukprn], [IfateReferenceNumber])
);
GO

CREATE UNIQUE INDEX IXU_StandardProviderQAR_TimePeriod ON [dbo].[StandardProviderQAR]
( [TimePeriod], [Ukprn], [IfateReferenceNumber] ) INCLUDE ( [Leavers], [AchievementRate], [AchievementRank] );
GO

