CREATE TABLE [dbo].[ProviderApprenticeStars]
(
    [Ukprn] [bigint] NOT NULL,
    [ReviewCount] [int] NOT NULL,
    [Stars] [int] NOT NULL,
    [TimePeriod] NVARCHAR(50) NOT NULL
);
GO

CREATE INDEX IXU_ProviderApprenticeStars ON [dbo].[ProviderApprenticeStars] ([Ukprn],[TimePeriod]) 
INCLUDE ([ReviewCount],[Stars]);
GO
