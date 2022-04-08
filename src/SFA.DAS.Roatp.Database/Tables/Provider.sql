CREATE TABLE [dbo].[Provider]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ExternalId] UNIQUEIDENTIFIER NOT NULL,
    [Ukprn] INT NOT NULL,
    [LegalName] VARCHAR(1000) NOT NULL,
    [TradingName] VARCHAR(1000) NULL,
    [Email] VARCHAR(300) NULL,
    [Phone] VARCHAR(20) NULL,
    [Website] VARCHAR(500) NULL,
    [MarketingInfo] VARCHAR(MAX) NULL,
    [EmployerSatisfaction] DECIMAL NULL,
    [HasConfirmedDetails] BIT NULL,
    [ConfirmedDetailsOn] datetime2(7) NULL,
    [LearnerSatisfaction] DECIMAL NULL,
    CONSTRAINT PK_Provider PRIMARY KEY (Id),
    CONSTRAINT UK_Provider_Id UNIQUE (ExternalId),
    CONSTRAINT UK_Provider_Ukprn UNIQUE (Ukprn)
)
