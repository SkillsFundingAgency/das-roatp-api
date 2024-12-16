CREATE TABLE [dbo].[ProviderQAR]
(
[TimePeriod] varchar(4),
[Ukprn] bigint,
[Leavers] varchar(10),
[AchievementRate] varchar(10),
[CreatedDate] Datetime2 DEFAULT GETUTCDATE()
);
GO

CREATE UNIQUE INDEX [IXU_ProviderQAR] ON [dbo].[ProviderQAR] ([TimePeriod],[Ukprn]) 
INCLUDE ([Leavers], [AchievementRate]);
GO
