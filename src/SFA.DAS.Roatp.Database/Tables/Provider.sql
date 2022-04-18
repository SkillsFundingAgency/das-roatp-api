CREATE TABLE [dbo].[Provider]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Ukprn] INT NOT NULL,
    [LegalName] VARCHAR(1000) NOT NULL,
    [TradingName] VARCHAR(1000) NULL,
    [Email] VARCHAR(300) NULL,
    [Phone] VARCHAR(50) NULL,
    [Website] VARCHAR(500) NULL,
    [MarketingInfo] VARCHAR(MAX) NULL,
    [EmployerSatisfaction] DECIMAL NULL,
    [LearnerSatisfaction] DECIMAL NULL,
    [ConfirmedDetailsOn] datetime2(7) NULL,
    [IsImported] BIT NOT NULL DEFAULT 0,
    [HasConfirmedLocations] BIT NULL,
    [HasConfirmedDetails] BIT NULL,

    CONSTRAINT PK_Provider PRIMARY KEY (Id),
    CONSTRAINT UK_Provider_Ukprn UNIQUE (Ukprn)
)
