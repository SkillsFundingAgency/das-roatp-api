CREATE TABLE [dbo].[ProviderAddress]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProviderId] INT NOT NULL, 
    [AddressLine1] VARCHAR(250) NULL, 
    [AddressLine2] VARCHAR(250) NULL,
    [AddressLine3] VARCHAR(250) NULL,
    [AddressLine4] VARCHAR(250) NULL, 
    [Town] VARCHAR(250) NULL, 
    [Postcode] VARCHAR(25) NULL, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL, 
    [AddressUpdateDate] DATETIME2 NOT NULL, 
    [CoordinatesUpdateDate] DATETIME2 NULL,
    CONSTRAINT PK_ProviderAddress PRIMARY KEY (Id),
    CONSTRAINT FK_ProviderAddress_Provider FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id),
    CONSTRAINT UK_ProviderAddress_ProviderId UNIQUE ([ProviderId])
)
