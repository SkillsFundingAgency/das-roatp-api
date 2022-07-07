CREATE TABLE [dbo].[ProviderLocation]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ImportedLocationId] INT NULL,
    [NavigationId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderId] INT NOT NULL,
    [RegionId] INT NULL,
    [LocationName] VARCHAR(250) NULL,
    [AddressLine1] VARCHAR(250) NULL,
    [AddressLine2] VARCHAR(250) NULL,
    [Town] VARCHAR(250) NULL,
    [Postcode] VARCHAR(25) NULL,
    [County] VARCHAR(250) NULL,
    [Latitude] FLOAT NOT NULL,
    [Longitude] FLOAT NOT NULL,
    [Email] VARCHAR(300) NULL,
    [Website] VARCHAR(500) NULL,
    [Phone] VARCHAR(50) NULL,
    [IsImported] BIT NOT NULL DEFAULT 0,
    [LocationType] INT NOT NULL, 
    CONSTRAINT PK_ProviderLocation PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderLocation_NavigationId UNIQUE ([NavigationId]),
    CONSTRAINT FK_ProviderLocation_Provider FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id)
)
