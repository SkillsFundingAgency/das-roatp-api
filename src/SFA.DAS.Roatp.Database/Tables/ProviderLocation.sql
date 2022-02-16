CREATE TABLE [dbo].[ProviderLocation]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ProviderId] UNIQUEIDENTIFIER NOT NULL,
    [Name] VARCHAR(250) NULL,
    [Email] VARCHAR(256) NULL,
    [Website] VARCHAR(256) NULL,
    [Phone] VARCHAR(50) NULL,
    [Address1] VARCHAR(250) NULL,
    [Address2] VARCHAR(250) NULL,
    [Town] VARCHAR(250) NULL,
    [Postcode] VARCHAR(25) NULL,
    [County] VARCHAR(250) NULL,
    [Latitude] FLOAT NULL,
    [Longitude] FLOAT NULL,
    [Radius] INT NULL,
    CONSTRAINT PK_ProviderLocation PRIMARY KEY (Id),
    CONSTRAINT FK_Provider_ProviderLocation FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id)
)
