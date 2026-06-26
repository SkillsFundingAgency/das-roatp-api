CREATE PROCEDURE [dbo].[DeleteExpiredShortlists]
    @expiryInDays int = 30
AS
BEGIN

    -- the recently active users, where oldest shortlist entries may be > @expiryDays
    SELECT st1.[UserId]
    INTO #ActiveUsers
    FROM [dbo].[Shortlist] st1
    JOIN [dbo].[ProviderCourse] pc1 on st1.[Larscode] = pc1.[LarsCode] 
    JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId AND pr1.Ukprn = st1.[Ukprn]
    JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[StatusId] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
    JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
    JOIN [dbo].[Standard] sd1 on sd1.LarsCode = st1.[Larscode]
    GROUP BY st1.[UserId]
    HAVING MAX(st1.CreatedDate) > CAST(DATEADD(day,0-@expiryInDays,GETUTCDATE()) AS DATE)
    ;

    CREATE UNIQUE INDEX IXU_ActiveUsers ON #ActiveUsers ([UserId]);

    DELETE FROM [dbo].[Shortlist] 
    WHERE [UserId] IN (
    -- all Inactive Users
        SELECT sht.[UserId] 
        FROM [dbo].[Shortlist] sht
        LEFT JOIN #ActiveUsers act on act.[UserId] = sht.[UserId]
        WHERE act.[UserId] IS NULL
    );


    -- where shortlist is for an active provider / standard
    SELECT st1.[Id]
    INTO #ActiveCourses
    FROM [dbo].[Shortlist] st1
    JOIN [dbo].[ProviderCourse] pc1 on st1.[Larscode] = pc1.[LarsCode] 
    JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId AND pr1.Ukprn = st1.[Ukprn]
    JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND (tp.[StatusId] = 1 AND tp.[ProviderTypeId] = 1) -- Active, Main only
    JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
    JOIN [dbo].[ProviderLocation] pl1 on pl1.Id = pcl1.ProviderLocationId
    JOIN [dbo].[Standard] sd1 on sd1.LarsCode = st1.[Larscode]
    GROUP BY st1.[Id]
    ;

    CREATE UNIQUE INDEX IXU_ActiveCourses ON #ActiveCourses ([Id]);

    DELETE FROM [dbo].[Shortlist]
    WHERE [Id] IN (
    -- shortlist entries for inactive courses
        SELECT sht.[Id]
        FROM [dbo].[Shortlist] sht
        LEFT JOIN #ActiveCourses act ON act.[Id] = sht.[Id]
        WHERE act.[Id] IS NULL
    );


    DROP TABLE #ActiveCourses;
    DROP TABLE #ActiveUsers;


END
