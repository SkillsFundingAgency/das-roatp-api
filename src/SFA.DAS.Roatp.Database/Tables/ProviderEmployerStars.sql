CREATE TABLE [dbo].[ProviderEmployerStars] (
    [TimePeriod] NVARCHAR(50) NOT NULL,
    [Ukprn] BIGINT NOT NULL,
    [ReviewCount] INT NOT NULL,
    [Stars] INT NOT NULL,
    [Rating] AS CONVERT(VARCHAR(20)
               ,CASE [Stars] WHEN 4 THEN 'Excellent' 
                             WHEN 2 THEN 'Poor' 
                             WHEN 3 THEN 'Good' 
                             WHEN 1 THEN 'VeryPoor' END)
);
GO

CREATE UNIQUE INDEX IXU_ProviderEmployerStars_TimePeriod ON [dbo].[ProviderEmployerStars]
( [TimePeriod], [Ukprn] ) INCLUDE ( [ReviewCount], [Stars], [Rating]);
GO
