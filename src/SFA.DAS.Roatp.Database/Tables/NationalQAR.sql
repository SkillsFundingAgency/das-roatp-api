CREATE TABLE [dbo].[NationalQAR]
(
[Timeperiod] varchar(4),
[Leavers] varchar(10),
[AchievementRate] varchar(10),
[CreatedDate] Datetime2 DEFAULT GETUTCDATE()
);
GO

CREATE UNIQUE INDEX [IXU_NationalQAR] ON [dbo].[NationalQAR] ([TimePeriod]) 
INCLUDE ([Leavers], [AchievementRate]);
GO
