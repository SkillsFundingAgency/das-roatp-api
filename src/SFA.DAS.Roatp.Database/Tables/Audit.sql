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
	ProviderId AS JSON_VALUE(InitialState, '$.ProviderId') PERSISTED,
	LarsCode   AS JSON_VALUE(InitialState, '$.LarsCode') PERSISTED,
	CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE INDEX [IX_Audit_EntityId] ON [Audit] ([EntityId])
GO

CREATE NONCLUSTERED INDEX IX_Audit_CreateProviderCourse
ON dbo.Audit (ProviderId, LarsCode, AuditDate)
WHERE UserAction = 'CreateProviderCourse';
GO

CREATE NONCLUSTERED INDEX IX_Audit_DeleteProviderCourse
ON dbo.Audit (ProviderId, LarsCode, AuditDate)
WHERE UserAction = 'DeleteProviderCourse';
GO