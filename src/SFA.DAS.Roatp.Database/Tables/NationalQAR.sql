CREATE TABLE [dbo].[NationalQAR]
(
  [TimePeriod] VARCHAR(4) NOT NULL,
  [Leavers] VARCHAR(10) NOT NULL,
  [AchievementRate] VARCHAR(10) NOT NULL,
  [CreatedDate] DATETIME2  NOT NULL DEFAULT GETUTCDATE(),
  CONSTRAINT PK_NationalQAR PRIMARY KEY ([TimePeriod])
);
GO
