CREATE TABLE [dbo].[Audit]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1),
	[CorrelationId] [uniqueidentifier] NULL,
	[EntityType] NVARCHAR(256) NOT NULL,
	[EntityId] NVARCHAR(256) NULL,
	[UserId] NVARCHAR(256) NOT NULL,
	[UserDisplayName] NVARCHAR(256) NULL,
	[UserAction] NVARCHAR(256) NOT NULL,
	[AuditDate] DATETIME2 NOT NULL,
	[InitialState] [nvarchar](max) NULL,
	[UpdatedState] [nvarchar](max) NULL,
	CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE INDEX [IX_Audit_EntityId] ON [Audit] ([EntityId])
GO