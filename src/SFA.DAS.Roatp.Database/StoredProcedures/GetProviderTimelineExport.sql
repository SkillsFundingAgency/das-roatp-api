CREATE PROCEDURE GetProviderTimelineExport 
    @ukprn int = null
AS
BEGIN

  -- courses by provider and type
  WITH
  ProviderAndCourseAndType
  AS
  (
    SELECT prv.[Ukprn], pcv.[LarsCode], std.[CourseType]
    ,CONVERT(date,
          CASE std.[LearningType]
          WHEN 'Apprenticeship' THEN '2014-08-01'
          WHEN 'FoundationApprenticeship' THEN'2025-08-01'
          WHEN 'ApprenticeshipUnit' THEN '2026-04-28'
          ELSE '2014-08-01'
          END) [EffectiveFrom]
    , pcv.[EffectiveTo], pac.[LastDateStarts], pct.[IsRestrictedProvider]
    FROM [ProviderCoursesTimelineView] pcv
    JOIN [dbo].[Provider] prv on prv.[Id] = pcv.[ProviderId]
    JOIN [dbo].[Standard] std on std.[LarsCode] = pcv.[LarsCode]
    JOIN [dbo].[ProviderCourseType] pct on std.[CourseType] = pct.[CourseType] and pct.[Ukprn] = prv.[Ukprn] 
    LEFT JOIN [dbo].[ProviderAllowedCourse] pac on pac.[LarsCode] = pcv.[LarsCode] and pac.[Ukprn] = prv.[Ukprn]
    WHERE 
    (CASE WHEN pct.[IsRestrictedProvider] = 0 
          THEN 1   -- Is not restricted, include all courses
          WHEN pac.[Id] IS NOT NULL 
          THEN 1   -- otherwise, is restricted, so needs a ProviderAllowedCourse record
          ELSE 0   -- so exclude where is restricted and does not have a ProviderAllowedCourse record
          END) = 1
  )

  SELECT * 
  FROM (
--  Main Providers - need all courses
    SELECT prd.[Ukprn], prd.[StatusId], prd.[ProviderTypeId], pad.[CourseType], pad.[LarsCode], pad.[EffectiveFrom], pad.[EffectiveTo], pad.[LastDateStarts]
    FROM [dbo].[ProviderRegistrationDetail] prd
    JOIN ProviderAndCourseAndType pad on pad.[Ukprn] = prd.[Ukprn]
    WHERE prd.[ProviderTypeId] = 1 AND prd.[StatusId] in (1,2)  -- Main provider, active or active not taking on apprentices

    UNION ALL

--  Employer Providers - just need course types
    SELECT pd2.[Ukprn], pd2.[StatusId], pd2.[ProviderTypeId], pct.[CourseType], null [LarsCode], null [EffectiveFrom], null [EffectiveTo], null [LastDateStarts]
    FROM [dbo].[ProviderRegistrationDetail] pd2 
    JOIN [dbo].[ProviderCourseType] pct on pct.Ukprn = pd2.Ukprn
    WHERE providertypeid = 2 and [StatusId] IN (1,2)

    UNION ALL

--  Supporting providers - just need provider type
    SELECT pd3.[Ukprn], pd3.[StatusId], pd3.[ProviderTypeId], null [CourseType], null [LarsCode], null [EffectiveFrom], null [EffectiveTo], null [LastDateStarts]
    FROM [dbo].[ProviderRegistrationDetail] pd3
    WHERE providertypeid = 3 and [StatusId] IN (1,2)
  ) ab1
  WHERE @ukprn IS NULL OR @ukprn = [Ukprn]
  ORDER BY [Ukprn], [LarsCode];

END