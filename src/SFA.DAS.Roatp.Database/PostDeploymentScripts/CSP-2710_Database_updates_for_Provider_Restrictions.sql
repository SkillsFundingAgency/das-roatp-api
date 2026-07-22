-- CSP-2710 Database updates for Provider Restrictions

-- back-populate Ukprn in ProviderCourse
MERGE INTO [dbo].[ProviderCourse] pac
USING
(
  SELECT pc1.[Id] , prv.[Ukprn]
  FROM [dbo].[ProviderCourse] pc1
  JOIN [dbo].[Provider] prv on prv.[Id] = pc1.[ProviderId]
) upd
ON (pac.[Id] = upd.[Id])
WHEN MATCHED AND pac.[Ukprn] IS NULL THEN
UPDATE SET pac.[Ukprn] = upd.[Ukprn]
;

-- preset IsRestrictedProvider in ProviderCourseType
UPDATE [dbo].[ProviderCourseType]
SET [IsRestrictedProvider] = 0
WHERE [IsRestrictedProvider] IS NULL
;
