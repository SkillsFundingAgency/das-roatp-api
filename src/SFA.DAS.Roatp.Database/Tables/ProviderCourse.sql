﻿CREATE TABLE [dbo].[ProviderCourse]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProviderId] INT NOT NULL,
    [LarsCode] INT NOT NULL,
    [StandardInfoUrl] VARCHAR(500) NOT NULL,
    [ContactUsPageUrl] VARCHAR(500) NULL,
    [ContactUsEmail] VARCHAR(300) NULL,
    [ContactUsPhoneNumber] VARCHAR(50) NULL,
    [IsApprovedByRegulator] BIT NULL,
    [IsImported] BIT NOT NULL DEFAULT 0,
    [HasPortableFlexiJobOption] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT PK_ProviderCourse PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderCourse_ProviderId_LarsCode UNIQUE (ProviderId, LarsCode),
    CONSTRAINT FK_ProviderCourse_Provider FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id)
)
