-- cleanup Managing Standards
-- 
BEGIN TRY
BEGIN TRANSACTION Cleanup;

-- Find courses without locations
SELECT Id, Providerid ,LarsCode 
INTO #ProviderCoursesMissingLocations
FROM (

SELECT pcs.* FROM [dbo].[ProviderCourse] pcs
LEFT JOIN [dbo].[ProviderCourseLocation] pcl  on pcs.Id = pcl.[ProviderCourseId]
WHERE pcl.[Id] IS NULL

) ab1

-- remove records from ProviderCourseVersion (if any)

SELECT * FROM [dbo].[ProviderCourseVersion]
WHERE ProviderCourseId IN (SELECT Id FROM #ProviderCoursesMissingLocations);

DELETE FROM [dbo].[ProviderCourseVersion]
WHERE ProviderCourseId IN (SELECT Id FROM #ProviderCoursesMissingLocations);

SELECT * FROM [dbo].[ProviderCourseLocation]
WHERE [ProviderCourseId] IN (SELECT Id FROM #ProviderCoursesMissingLocations);

SELECT * FROM [dbo].[ProviderCourse]
WHERE [Id] IN (SELECT Id FROM #ProviderCoursesMissingLocations);

-- Remove orphaned Provider Courses with no locations
DELETE FROM [dbo].[ProviderCourse]
WHERE [Id] IN (SELECT Id FROM #ProviderCoursesMissingLocations);



COMMIT TRANSACTION Cleanup;

DROP TABLE #ProviderCoursesMissingLocations;


END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION Cleanup;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH

