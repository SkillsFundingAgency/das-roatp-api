CREATE TABLE [dbo].[Audit]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1),
	[CorrelationId] [uniqueidentifier] NULL,
	[EntityType] NVARCHAR(256) NOT NULL,
	[EntityId] BIGINT NOT NULL,
	[UserId] NVARCHAR(256),
	[UserDisplayName] NVARCHAR(256),
	[UserAction] NVARCHAR(256),
	[AuditDate] DATETIME2,
	[InitialState] [nvarchar](max) NULL,
	[UpdatedState] [nvarchar](max) NULL,
	CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE INDEX [IX_Audit_EntityId] ON [Audit] ([EntityId])
GO