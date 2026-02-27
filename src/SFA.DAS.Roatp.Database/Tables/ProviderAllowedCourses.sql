CREATE TABLE [dbo].[ProviderAllowedCourses]
(
  [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  [Ukprn] INT NOT NULL,
  [LarsCode] VARCHAR(20) NOT NULL
);

GO;

CREATE UNIQUE INDEX IX_ProviderAllowedCourses_Ukprn_LarsCode
ON [dbo].[ProviderAllowedCourses] ([Ukprn],[LarsCode]);

GO;


