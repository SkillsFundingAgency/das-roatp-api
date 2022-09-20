CREATE TABLE [dbo].[ImportAudit]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[TimeStarted] DATETIME NOT NULL,
	[TimeFinished] DATETIME NOT NULL,
	[RowsImported] INT NOT NULL,
	[ImportType] varchar(50) NOT NULL,

	CONSTRAINT PK_ImportAudit PRIMARY KEY (Id)
)
GO