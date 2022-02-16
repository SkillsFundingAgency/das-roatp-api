CREATE TABLE [dbo].[Provider]
(
    [Id] UNIQUEIDENTIFIER NOT NULL, 
    [Ukprn] INT NOT NULL,
    [LegalName] VARCHAR(1000) NOT NULL,
    [TradingName] VARCHAR(1000) NULL,
    [Email] VARCHAR(256) NULL,
    [Phone] VARCHAR(50) NULL,
    [Website] VARCHAR(500) NULL,
    [MarketingInfo] VARCHAR(MAX) NULL,
    [EmployerSatisfaction] DECIMAL NULL,
    [LearnerSatisfaction] DECIMAL NULL,
    CONSTRAINT PK_Provider PRIMARY KEY (Id)
)
