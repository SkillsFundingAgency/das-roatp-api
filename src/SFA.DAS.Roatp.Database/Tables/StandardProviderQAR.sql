CREATE TABLE [dbo].[StandardProviderQAR]
(
  [TimePeriod] VARCHAR(4), 
  [IfateReferenceNumber] VARCHAR(10),
  [Ukprn] bigint, 
  [Leavers] VARCHAR(10), 
  [AchievementRate] VARCHAR(10),
  [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
  CONSTRAINT PK_StandardProviderQAR PRIMARY KEY ([TimePeriod], [Ukprn], [IfateReferenceNumber])
);
GO
