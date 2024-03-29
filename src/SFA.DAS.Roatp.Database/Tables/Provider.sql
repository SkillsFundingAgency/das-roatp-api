﻿CREATE TABLE [dbo].[Provider]
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
    [IsImported] BIT NOT NULL DEFAULT 0,

    CONSTRAINT PK_Provider PRIMARY KEY (Id),
    CONSTRAINT UK_Provider_Ukprn UNIQUE (Ukprn),
    INDEX IX_Provider_Ukrpn NONCLUSTERED (Ukprn)
)
