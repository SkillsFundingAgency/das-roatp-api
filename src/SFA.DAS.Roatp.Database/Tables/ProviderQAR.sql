CREATE TABLE [dbo].[ProviderQAR]
(
  [TimePeriod] VARCHAR(4) NOT NULL,
  [Ukprn] BIGINT  NOT NULL,
  [Leavers] VARCHAR(10) NOT NULL,
  [AchievementRate] VARCHAR(10) NOT NULL,
  [CreatedDate] DATETIME2  NOT NULL DEFAULT GETUTCDATE(),
  CONSTRAINT PK_ProviderQAR PRIMARY KEY ([TimePeriod], [Ukprn])
);
GO

