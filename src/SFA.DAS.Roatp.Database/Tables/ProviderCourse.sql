CREATE TABLE [dbo].[ProviderCourse]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ExternalId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderId] INT NOT NULL,
    [LarsCode] INT NOT NULL,
    [IfateReferenceNumber] VARCHAR(10) NOT NULL,
    [StandardInfoUrl] VARCHAR(500) NOT NULL,
    [ContactUsPageUrl] VARCHAR(500) NULL,
    [ContactEmail] VARCHAR(300) NULL,
    [ContactPhoneNumber] VARCHAR(20) NULL,
    CONSTRAINT PK_ProviderCourse PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderCourse_Id UNIQUE ([ExternalId]),
    CONSTRAINT UK_ProviderCourse_ProviderIdLarsCode UNIQUE (ProviderId, LarsCode),
    CONSTRAINT FK_Provider_ProviderCourse FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id) 
)
