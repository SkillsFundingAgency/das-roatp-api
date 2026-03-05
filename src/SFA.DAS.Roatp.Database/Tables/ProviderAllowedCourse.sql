CREATE TABLE [dbo].[ProviderAllowedCourse]
(
  [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  [Ukprn] INT NOT NULL,
  [LarsCode] VARCHAR(20) NOT NULL
);

GO;

CREATE UNIQUE INDEX IX_ProviderAllowedCourse_Ukprn_LarsCode
ON [dbo].[ProviderAllowedCourse] ([Ukprn],[LarsCode]);

GO;


