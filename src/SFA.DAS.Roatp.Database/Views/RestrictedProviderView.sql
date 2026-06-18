CREATE VIEW [dbo].[RestrictedProviderView]
AS
    SELECT pct.[Id] [ProviderCourseTypeId]
    ,pct.[Ukprn]
    ,pct.[CourseType]
    ,CASE WHEN pac.[Id] IS NULL THEN 0 ELSE 1 END isProviderRestricted
    FROM [dbo].[ProviderCourseType] pct
    LEFT JOIN [dbo].[RestrictedProviderCourseType] pac 
    ON pac.[Ukprn] = pct.[Ukprn] AND pac.[CourseType] = pct.[CourseType]
;