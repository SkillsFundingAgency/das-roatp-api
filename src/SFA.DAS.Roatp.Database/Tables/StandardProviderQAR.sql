CREATE TABLE [dbo].[StandardProviderQAR]
(
  [TimePeriod] VARCHAR(4), 
  [IfateReferenceNumber] VARCHAR(10),
  [Ukprn] bigint, 
  [Leavers] VARCHAR(10), 
  [AchievementRate] VARCHAR(10),
  [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
  [AchievementRank] AS (CONVERT(VARCHAR(20)
                       ,CASE WHEN ISNULL([AchievementRate],'x') = 'x' 
                             THEN 'None'
                             WHEN [AchievementRate] < '50' THEN 'VeryPoor'
                             WHEN [AchievementRate] < '60' THEN 'Poor'
                             WHEN [AchievementRate] < '70' THEN 'Good'
                             ELSE 'Excellent' END)),    
  CONSTRAINT PK_StandardProviderQAR PRIMARY KEY ([TimePeriod], [Ukprn], [IfateReferenceNumber])
);
GO
