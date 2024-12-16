CREATE TABLE [dbo].[StandardProviderQAR]
(
[TimePeriod] VARCHAR(4), 
[IfateReferenceNumber] VARCHAR(10),
[Ukprn] bigint, 
[Leavers] VARCHAR(10), 
[AchievementRate] VARCHAR(10),
[CreatedDate] DATETIME2 DEFAULT GETUTCDATE()
);
GO

CREATE UNIQUE INDEX [IXU_StandardProviderQAR] ON [dbo].[StandardProviderQAR] ([Ukprn], [IfateReferenceNumber], [TimePeriod]) 
INCLUDE ([Leavers], [AchievementRate]);
GO
