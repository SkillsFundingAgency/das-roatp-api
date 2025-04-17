﻿CREATE TABLE [dbo].[StandardQAR]
(
  [TimePeriod] VARCHAR(4), 
  [IfateReferenceNumber] VARCHAR(10),
  [Leavers] VARCHAR(10), 
  [AchievementRate] VARCHAR(10),
  [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
  CONSTRAINT PK_StandardQAR PRIMARY KEY ([TimePeriod], [IfateReferenceNumber])
);
GO

CREATE INDEX IX_StandardQAR_TimePeriod ON [dbo].[StandardQAR]
( [TimePeriod], [IfateReferenceNumber] ) INCLUDE ( [Leavers], [AchievementRate] );
GO
