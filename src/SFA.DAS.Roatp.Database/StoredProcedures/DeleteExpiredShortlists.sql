CREATE PROCEDURE [dbo].[DeleteExpiredShortlists]
    @expiryInDays int
AS
BEGIN

    DELETE FROM [dbo].[Shortlist] 
    WHERE [UserId] IN (
        -- all users
        SELECT DISTINCT [UserId]
        FROM [dbo].[Shortlist] 
        EXCEPT
        -- the recently active users
        SELECT st1.[UserId]
        FROM [dbo].[Shortlist] st1
        JOIN [dbo].[ProviderCourse] pc1 on st1.[Larscode] = pc1.[LarsCode] 
        JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId AND pr1.Ukprn = st1.[Ukprn]
        JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
        JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
        JOIN [dbo].[Standard] sd1 on sd1.LarsCode = st1.[Larscode]
        GROUP BY st1.[UserId]
        HAVING MAX(st1.CreatedDate) > DATEADD(day,0-@expiryInDays,GETUTCDATE())
    )
    OR [Id] IN (
        SELECT [Id] FROM (
            -- all remaining shortlist entries
            SELECT [Id]
            FROM [dbo].[Shortlist]
            EXCEPT
            -- where shortlist is for an active provider / standard
            SELECT st1.[Id]
            FROM [dbo].[Shortlist] st1
            JOIN [dbo].[ProviderCourse] pc1 on st1.[Larscode] = pc1.[LarsCode] 
            JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId AND pr1.Ukprn = st1.[Ukprn]
            JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND (tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1) -- Active, Main only
            JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
            JOIN [dbo].[ProviderLocation] pl1 on pl1.Id = pcl1.ProviderLocationId
            JOIN [dbo].[Standard] sd1 on sd1.LarsCode = st1.[Larscode]
            GROUP BY st1.[Id], st1.[LarsCode]
        ) ab1
    )

END
