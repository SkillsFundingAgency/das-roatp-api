CREATE TABLE [dbo].[ProviderEmployerStars] (
    [Ukprn] [bigint] NOT NULL,
    [ReviewCount] [int] NOT NULL,
    [Stars] [int] NOT NULL,
    [TimePeriod] NVARCHAR(50) NOT NULL
);
GO

CREATE INDEX IXU_ProviderEmployerStars ON [dbo].[ProviderEmployerStars] ([Ukprn],[TimePeriod]) 
INCLUDE ([ReviewCount],[Stars]);
GO
