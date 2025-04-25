CREATE TABLE [dbo].[ProviderRegistrationDetail]
(
    [Ukprn] INT NOT NULL PRIMARY KEY,
    [LegalName] VARCHAR(1000) NOT NULL,
    [StatusId] INT NOT NULL,
    [StatusDate] DATETIME2 NOT NULL,
    [OrganisationTypeId] INT NOT NULL,
    [ProviderTypeId] INT NOT NULL,
    [AddressLine1] VARCHAR(250) NULL,
    [AddressLine2] VARCHAR(250) NULL,
    [AddressLine3] VARCHAR(250) NULL,
    [AddressLine4] VARCHAR(250) NULL,
    [Town] VARCHAR(250) NULL,
    [Postcode] VARCHAR(25) NULL,
    [Latitude] FLOAT NULL,
    [Longitude] FLOAT NULL
);
GO

CREATE NONCLUSTERED INDEX IX_ProviderRegistrationDetail_StatusId_ProviderTypeId
ON [dbo].[ProviderRegistrationDetail] ([Ukprn],[StatusId],[ProviderTypeId])
INCLUDE ([LegalName]);
GO

