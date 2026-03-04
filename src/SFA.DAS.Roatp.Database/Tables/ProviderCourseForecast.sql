CREATE TABLE [dbo].[ProviderCourseForecast]
(
  [Id] INT IDENTITY(1,1) NOT NULL,
  [Ukprn] INT NOT NULL,
  [LarsCode] NVARCHAR(10) NOT NULL,
  [TimePeriod] NVARCHAR(6) NOT NULL,
  [Quarter] INT NOT NULL,
  [EstimatedLearners] INT NULL,
  [CreatedDate] DATETIME2 DEFAULT GETUTCDATE(),
  [UpdatedDate] DATETIME2 DEFAULT GETUTCDATE()
  CONSTRAINT PK_ProviderCoursesForecast PRIMARY KEY ([Id]),
);
GO

CREATE UNIQUE INDEX IXU_ProviderCourseForecast
ON [dbo].[ProviderCourseForecast] ([Ukprn],[LarsCode],[TimePeriod],[Quarter])
INCLUDE ([EstimatedLearners],[CreatedDate],[UpdatedDate]);
GO
