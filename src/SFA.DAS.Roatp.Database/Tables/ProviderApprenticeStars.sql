CREATE TABLE [dbo].[ProviderApprenticeStars]
(
    [Ukprn] BIGINT NOT NULL,
    [ReviewCount] INT NOT NULL,
    [Stars] INT NOT NULL,
    [TimePeriod] NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_ProviderApprenticeStars PRIMARY KEY ([TimePeriod], [Ukprn])
);
GO
