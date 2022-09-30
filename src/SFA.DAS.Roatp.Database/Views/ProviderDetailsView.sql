CREATE VIEW [dbo].[ProviderDetailsView]
  AS 
  SELECT 
    P.Id,
    P.Ukprn,
    P.LegalName,
    P.TradingName,
    P.MarketingInfo,
    P.Email,
    P.Phone,
    P.Website,
    A.AddressLine1,
    A.AddressLine2,
    A.AddressLine3,
    A.AddressLine4,
    A.Town,
    A.Postcode,
    A.Latitude,
    A.Longitude
  FROM [dbo].[Provider] P
  LEFT OUTER JOIN [dbo].[ProviderAddress] A on P.Id = A.ProviderId
