﻿CREATE TABLE [dbo].[ProviderLocation]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ExternalId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderId] INT NOT NULL,
    [LocationName] VARCHAR(250) NULL,
    [Email] VARCHAR(300) NULL,
    [Website] VARCHAR(500) NULL,
    [Phone] VARCHAR(20) NULL,
    [AddressLine1] VARCHAR(250) NULL,
    [AddressLine2] VARCHAR(250) NULL,
    [Town] VARCHAR(250) NULL,
    [Postcode] VARCHAR(25) NULL,
    [County] VARCHAR(250) NULL,
    [Latitude] FLOAT NULL,
    [Longitude] FLOAT NULL,
    [Radius] INT NULL,
    CONSTRAINT PK_ProviderLocation PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderLocation_Id UNIQUE ([ExternalId]),
    CONSTRAINT FK_Provider_ProviderLocation FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id)
)
