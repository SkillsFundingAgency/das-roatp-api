CREATE TABLE [dbo].[ProviderRegistrationDetail]
(
    [Ukprn] INT NOT NULL PRIMARY KEY, 
    [LegalName] VARCHAR(1000) NOT NULL,
    [StatusId] INT NOT NULL, 
    [StatusDate] DATETIME2 NOT NULL, 
    [OrganisationTypeId] INT NOT NULL, 
    [ProviderTypeId] INT NOT NULL, 
)
