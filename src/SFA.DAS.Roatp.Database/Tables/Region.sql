CREATE TABLE [dbo].[Region]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[SubregionName] VARCHAR(250) NULL,
	[RegionName] VARCHAR(25) NULL,
	[Latitude] FLOAT NOT NULL,
    [Longitude] FLOAT NOT NULL
);

GO

CREATE UNIQUE INDEX IDX_SubregionName
   ON Region (SubregionName);   
GO
   
CREATE INDEX IX_Regions ON [dbo].[Region]
( [Id], [Latitude], [Longitude] ) INCLUDE ( [RegionName] , [SubregionName]);
GO
