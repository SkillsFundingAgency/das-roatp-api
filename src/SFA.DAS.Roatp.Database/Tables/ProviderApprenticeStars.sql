CREATE TABLE [dbo].[ProviderApprenticeStars]
(
    [Ukprn] BIGINT NOT NULL,
    [ReviewCount] INT NOT NULL,
    [Stars] INT NOT NULL,
    [TimePeriod] NVARCHAR(50) NOT NULL,
    [Rating] AS CONVERT(VARCHAR(20)
               ,CASE [Stars] WHEN 4 THEN 'Excellent' 
                             WHEN 2 THEN 'Poor' 
                             WHEN 3 THEN 'Good' 
                             WHEN 1 THEN 'VeryPoor' END),
   --CONSTRAINT PK_ProviderApprenticeStars PRIMARY KEY ([TimePeriod], [Ukprn])
);
GO

CREATE UNIQUE INDEX IXU_ProviderApprenticeStars_TimePeriod ON [dbo].[ProviderApprenticeStars]
( [TimePeriod], [Ukprn] ) INCLUDE ( [ReviewCount], [Stars], [Rating]);
GO
