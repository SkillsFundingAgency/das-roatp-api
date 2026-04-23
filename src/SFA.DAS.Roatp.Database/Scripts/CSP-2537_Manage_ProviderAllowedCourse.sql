-- Script to add new providers for short courses, and to add or remove courses allowed for providers to das-prd-roatpv2-db
-- Will only add a course for a provider where the provider is authorised for the CourseType 
--
BEGIN TRANSACTION PAC;
BEGIN

CREATE TABLE #ProviderAndCourse
(
[Ukprn] bigint not null,
[LarsCode] varchar(20) not null,
[Allowed] bit not null
);
CREATE UNIQUE INDEX IXU_ProviderAndCourse ON #ProviderAndCourse ([Ukprn],[LarsCode] );

-- add setting here to allow [Allowed]=1 ( or to later disallow [Allowed]=0 )
INSERT INTO  #ProviderAndCourse VALUES
----------------------------------------------------------------------------
-- EXAMPLES (to be removed)
----------------------------------------------------------------------------
(10000020,'ZSC00002',1),
(10000028,'ZSC00001',1),
(10000488,'ZSC00002',1),
(10000239,'ZSC00004',0),
(10000020,'ZSC00009',1),
(10000028,'ZSC00009',1),
(10000488,'ZSC00009',1),
(10002111,'ZSC00009',1),
(10090480,'ZSC00009',1),
(10000427,'ZSC00007',1);

----------------------------------------------------------------------------
-- EXAMPLES
----------------------------------------------------------------------------
;

SELECT COUNT(*) 'Before: Providers Approved for Short Courses ' FROM [dbo].[ProviderCourseType] WHERE [CourseType] = 'ShortCourse';
SELECT * FROM [dbo].[ProviderCourseType] WHERE [CourseType] = 'ShortCourse'  ORDER BY 1;


-- Add any new Approved providers for shortcourses
INSERT INTO [dbo].[ProviderCourseType] ([Ukprn],[CourseType])
SELECT DISTINCT tmp.[Ukprn], 'ShortCourse'
FROM #ProviderAndCourse tmp
WHERE 1=1
AND NOT EXISTS (SELECT null FROM  [dbo].[ProviderCourseType] WHERE [ukprn] = tmp.[Ukprn] AND [CourseType] = 'ShortCourse')
;

SELECT COUNT(*) 'After: Providers Approved for Short Courses ' FROM [dbo].[ProviderCourseType] WHERE [CourseType] = 'ShortCourse';
SELECT * FROM [dbo].[ProviderCourseType] WHERE [CourseType] = 'ShortCourse' ORDER BY 1;


SELECT COUNT(*) 'Before: Provider Courses Allowed for Apprenticeship Units ' FROM [dbo].[ProviderAllowedCourse];
SELECT * FROM [dbo].[ProviderAllowedCourse];


-- Add any new Allowed providers and courses
MERGE INTO [dbo].[ProviderAllowedCourse] pac
USING (
SELECT tmp.[Ukprn], tmp.[LarsCode] 
FROM #ProviderAndCourse tmp
JOIN [dbo].[Standard] st1 on st1.[LarsCode]  = tmp.[LarsCode]
JOIN [dbo].[ProviderCourseType] pct on pct.[courseType] = st1.[CourseType] AND pct.[Ukprn] = tmp.[Ukprn]
WHERE tmp.[Allowed] = 1
) upd
ON (pac.[Ukprn] = upd.[Ukprn] and pac.[LarsCode] = upd.[LarsCode] )
WHEN NOT MATCHED THEN
INSERT VALUES ([Ukprn], [LarsCode])
;


-- remove any newly Disallowed providers and courses
MERGE INTO [dbo].[ProviderAllowedCourse] pac
USING (
SELECT tmp.[Ukprn], tmp.[LarsCode] 
FROM #ProviderAndCourse tmp
JOIN [dbo].[ProviderAllowedCourse] pc2 on pc2.[Ukprn] = tmp.[Ukprn] and pc2.[LarsCode] = tmp.[LarsCode] 
WHERE tmp.[Allowed] = 0
) del
ON (pac.[Ukprn] = del.[Ukprn] and pac.[LarsCode] = del.[LarsCode])
WHEN MATCHED THEN
DELETE
;

DROP TABLE #ProviderAndCourse;

SELECT COUNT(*) 'After: Provider Courses Allowed for Apprenticeship Units ' FROM [dbo].[ProviderAllowedCourse];
SELECT * FROM [dbo].[ProviderAllowedCourse];

COMMIT TRANSACTION PAC;

END;
