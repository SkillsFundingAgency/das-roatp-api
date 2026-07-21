CREATE TABLE [dbo].[Shortlist]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Ukprn] INT NOT NULL,
    [Larscode] NVARCHAR(10) NOT NULL,
    [LocationDescription] VARCHAR(1000) NULL,
    [Latitude] FLOAT NULL,
    [Longitude] FLOAT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Shortlist PRIMARY KEY ([Id]) 
);
GO

CREATE NONCLUSTERED INDEX [IX_Shortlist_UserItems] ON [dbo].[Shortlist] ([UserId],[CreatedDate]);
GO


CREATE UNIQUE NONCLUSTERED INDEX [IXU_Shortlist] ON [dbo].[Shortlist]
(
    [UserId],
    [Ukprn],
    [Larscode],
    [LocationDescription]
)
INCLUDE([Latitude],[Longitude]);
GO
