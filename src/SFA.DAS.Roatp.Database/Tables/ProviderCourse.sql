CREATE TABLE [dbo].[ProviderCourse]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [ProviderId] UNIQUEIDENTIFIER NOT NULL,
    [LarsCode] INT NOT NULL,
    [IfateReferenceNumber] VARCHAR(10) NOT NULL,
    [StandardInfoUrl] VARCHAR(500) NOT NULL,
    [ContactUsPageUrl] VARCHAR(500) NULL,
    [ContactEmail] VARCHAR(500) NULL,
    [ContactPhoneNumber] VARCHAR(20) NULL,
    CONSTRAINT PK_ProviderCourse PRIMARY KEY (Id),
    CONSTRAINT UK_ProviderCourse UNIQUE (ProviderId, LarsCode),
    CONSTRAINT FK_Provider_ProviderCourse FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id) 
)
