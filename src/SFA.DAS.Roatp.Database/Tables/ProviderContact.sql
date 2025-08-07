CREATE TABLE [dbo].[ProviderContact]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL, 
    [ProviderId] INT NOT NULL, 
    [EmailAddress] VARCHAR(300) NULL, 
    [PhoneNumber] VARCHAR(50) NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcDate(),
    CONSTRAINT PK_ProviderContact PRIMARY KEY (Id),
    CONSTRAINT FK_ProviderContact_Provider FOREIGN KEY (ProviderId) REFERENCES [Provider] (Id)
);
GO

CREATE NONCLUSTERED INDEX IX_ProviderContact_ProviderId_CreatedDate
ON [dbo].[ProviderContact] ([ProviderId],[CreatedDate])
INCLUDE ([EmailAddress],[PhoneNumber]);
GO
