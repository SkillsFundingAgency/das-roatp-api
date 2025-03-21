CREATE TABLE [dbo].[ProviderEmployerStars] (
    [TimePeriod] NVARCHAR(50) NOT NULL,
    [Ukprn] BIGINT NOT NULL,
    [ReviewCount] INT NOT NULL,
    [Stars] INT NOT NULL,
    CONSTRAINT PK_ProviderEmployerStars PRIMARY KEY ([TimePeriod], [Ukprn])
);
GO
