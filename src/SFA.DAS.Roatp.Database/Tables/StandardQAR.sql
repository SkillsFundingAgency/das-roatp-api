CREATE TABLE [dbo].[StandardQAR]
(
[TimePeriod] VARCHAR(4), 
[IfateReferenceNumber] VARCHAR(10),
[Leavers] VARCHAR(10), 
[AchievementRate] VARCHAR(10),
[CreatedDate] DATETIME2 DEFAULT GETUTCDATE()
);
GO

CREATE UNIQUE INDEX [IXU_StandardQAR] ON [dbo].[StandardQAR] ([IfateReferenceNumber], [TimePeriod]) 
INCLUDE ([Leavers], [AchievementRate]);
GO
