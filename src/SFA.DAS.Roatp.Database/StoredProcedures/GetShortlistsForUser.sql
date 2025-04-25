CREATE PROCEDURE GetShortListsForUser ( @userId uniqueidentifier )
AS
BEGIN

    -- the latest QAR data time period and Feedback reviews
    DECLARE @QARPeriod varchar(4), @ReviewPeriod varchar(6), @feedbackperiod varchar(10), @JSON varchar(max);

    -- the QAR Period is the last one for which we have data
    SELECT @QARPeriod = MAX([TimePeriod]) FROM [dbo].[NationalQAR];

    -- The Employer and Apprentice Review period is the previous AY
    SELECT @ReviewPeriod = RIGHT(YEAR(DATEADD(month,-19,getutcdate())),2)+RIGHT(YEAR(DATEADD(month,-7,getutcdate())),2);

    -- For Employer and Apprentice feedback time period is "AYxxxx"
    -- get latest full year we have data for - should be previous AY.
    SELECT @feedbackperiod = MAX([TimePeriod])
      FROM [dbo].[ProviderEmployerStars]
      WHERE [TimePeriod] LIKE 'AY%' AND [TimePeriod] <= 'AY'+@ReviewPeriod;

    IF @feedbackperiod IS NULL
        SET @feedbackperiod = 'AY'+@ReviewPeriod;
    ELSE
        SET @ReviewPeriod = REPLACE(@feedbackperiod,'AY','');

    WITH ShortlistedRegions
    AS
    -- The nearest regions to shortlisted locations
    (
        SELECT ShortlistId, NearestRegionId, AlternativeRegionid
        FROM (
            SELECT st1.[Id] ShortlistId, st1.locationDescription, reg1.[Id] NearestRegionId,  reg2.[id] AlternativeRegionid, 
            ROW_NUMBER() OVER (PARTITION BY st1.[Id] ORDER BY
                               geography::Point(reg1.Latitude, reg1.Longitude, 4326)
                               .STDistance(geography::Point(convert(float,st1.Latitude), convert(float,st1.Longitude), 4326)) ) seqn
                    
            FROM [dbo].[Region] reg1
            JOIN [dbo].[Shortlist] st1 ON st1.Latitude IS NOT NULL
            LEFT JOIN [dbo].[Region] reg2 ON reg1.[Latitude] = reg2.[Latitude] AND reg1.[Longitude]= reg2.[Longitude] AND reg1.[Id] != reg2.[id]
            WHERE st1.[userId] = @userId
        ) shtreg
        WHERE seqn = 1
    )
    -- Main query
    SELECT
         DENSE_RANK() OVER (ORDER BY stq.[Title], stq.[Level] , ab2.[larsCode] ) "ordering"
        ,ab2.larsCode larsCode
        ,stq.[Title]+' (level '+CONVERT(varchar,stq.[Level])+')' standardName
        ,stq.[IfateReferenceNumber]
        -- Training locations
        ,DENSE_RANK() OVER (PARTITION BY stq.[Title], stq.[Level], ab2.[larsCode] 
                            ORDER BY ISNULL(ab2.[LocationDescription],' ') ) "l2.ordering"
        ,LocationDescription 
        -- Training providers at eash location
        ,ROW_NUMBER() OVER (PARTITION BY stq.[Title], stq.[Level], ab2.[larsCode], ISNULL(ab2.[LocationDescription],' ') 
                            ORDER BY ab2.[LegalName] ) "l2.p3.ordering"
        ,ShortlistId
        ,CreatedDate
        ,ab2.Ukprn
        ,ab2.LegalName providerName
        ,AtEmployer
        ,BlockRelease
        ,BlockReleaseDistance
        ,BlockReleaseCount
        ,DayRelease
        ,DayReleaseDistance
        ,DayReleaseCount
        ,ContactUsEmail email
        ,ContactUsPhoneNumber phone
        ,ContactUsPageUrl website
    INTO #MainQuery        
    FROM
    (
        SELECT ShortlistId
            ,CreatedDate
            ,LocationDescription
            ,Ukprn
            ,LegalName
            ,ContactUsEmail 
            ,ContactUsPhoneNumber 
            ,ContactUsPageUrl
            ,larsCode
            ,CASE WHEN AtEmployer = 1 THEN 1 ELSE 0 END AtEmployer
            ,MAX(CASE WHEN BlockRelease = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]) BlockRelease
            ,MIN(CASE WHEN BlockRelease = 1 THEN Distance ELSE NULL END) OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]) BlockReleaseDistance
            ,SUM(CASE WHEN BlockRelease = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]) BlockReleaseCount
            ,MAX(CASE WHEN DayRelease = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]) DayRelease
            ,MIN(CASE WHEN DayRelease = 1 THEN Distance ELSE NULL END) OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]) DayReleaseDistance
            ,SUM(CASE WHEN DayRelease = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]) DayReleaseCount

            -- priority for at workplace over at provider (by ukprn and course)
            ,ROW_NUMBER() OVER (PARTITION BY ShortlistId, [Ukprn], [larsCode]
                                ORDER BY Distance) TP_Std_Dist_Seq
        FROM
            (
            -- base query
            SELECT st1.[Id] ShortlistId
                  ,st1.CreatedDate
                  ,st1.[userId]
                  ,st1.[larsCode]
                  ,st1.[Ukprn]
                  ,st1.LocationDescription 
                  ,pr1.LegalName
                  ,ISNULL(pc1.ContactUsEmail,pr1.Email) ContactUsEmail
                  ,ISNULL(pc1.ContactUsPhoneNumber, pr1.Phone) ContactUsPhoneNumber
                  ,ISNULL(pc1.ContactUsPageUrl, pr1.Website) ContactUsPageUrl
                  ,[LocationType]
                  -- LocationType: Provider = 0, National = 1, Regional = 2
                  ,CASE [LocationType] 
                   WHEN 0 THEN 0 ELSE 1 END AtEmployer
                  ,ISNULL(HasBlockReleaseDeliveryOption,0) BlockRelease
                  ,ISNULL(HasDayReleaseDeliveryOption,0) DayRelease
                  ,CASE
                   WHEN st1.Latitude IS NULL THEN 0  -- no distance check
                   WHEN [LocationType] = 0 THEN 2  -- Provider
                   WHEN [LocationType] = 1 THEN 1  -- National
                   WHEN pl1.[RegionId] IS NOT NULL THEN 
                        (CASE WHEN pl1.[RegionId] = NearestRegionId THEN 0 -- same Region
                              WHEN AlternativeRegionid IS NOT NULL AND pl1.[RegionId] = AlternativeRegionid THEN 0 -- alternative Region
                              ELSE 3 -- other Regions
                              END)
                   ELSE 3 -- other
                   END LocationOrdering
                   -- calculate distance 
                  ,CASE 
                   WHEN st1.Latitude IS NOT NULL AND [LocationType] = 0  -- Provider
                   THEN ROUND(geography::Point(pl1.Latitude, pl1.Longitude, 4326)
                             .STDistance(geography::Point(convert(float,st1.Latitude), convert(float,st1.Longitude), 4326)) * 0.0006213712,1) 
                   ELSE 0 END AS Distance
              FROM [dbo].[Shortlist] st1
                LEFT JOIN ShortlistedRegions slr on slr.[ShortlistId] = st1.[Id]
                JOIN [dbo].[ProviderCourse] pc1 on st1.[larsCode] = pc1.[larsCode] 
                JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId AND pr1.Ukprn = st1.[UkPrn]
                JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
                JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
                JOIN [dbo].[ProviderLocation] pl1 on pl1.Id = pcl1.ProviderLocationId
                WHERE 1=1
                AND [userId] = @userId
              ) ab1 
        WHERE 1=1
        AND LocationOrdering != 3 -- exclude outside Regions

        ) ab2
    -- Standards 
    JOIN [dbo].[Standard] stq on stq.larsCode = ab2.larsCode
    WHERE 1=1
    AND TP_Std_Dist_Seq = 1 -- just one row of result per shortlistId
    ;


    WITH
    -- the Standards and Provider QAR by Standard
    ProviderQARs
    AS
    (
        SELECT [Ukprn], qar.[IfateReferenceNumber], [Leavers], [AchievementRate]
        FROM [dbo].[StandardProviderQAR] qar
        WHERE [TimePeriod] = @QARPeriod
    )
    ,
    -- The Employer feedback
    EmployerStars
    AS
    (
        SELECT *
        FROM [dbo].[ProviderEmployerStars] 
        WHERE TimePeriod = @feedbackperiod
    ),
    -- The Apprentice feedback
    ApprenticeStars
    AS
    (
        SELECT *
        FROM [dbo].[ProviderApprenticeStars] 
        WHERE TimePeriod = @feedbackperiod
    )
    -- Prepare the JSON for response
    SELECT @JSON = (
        SELECT 
            toplevel.* 
            ,courses."ordering"
            ,courses.standardName
            ,courses.larsCode
            ,locations.ordering
            ,locations.locationDescription
            ,providers.ordering
            ,providers.shortlistId
            ,providers.ukprn
            ,providers.providerName
            ,providers.atEmployer
            ,providers.hasBlockRelease
            ,providers.blockReleaseDistance
            ,providers.blockReleaseCount
            ,providers.hasDayRelease
            ,providers.dayReleaseDistance
            ,providers.dayReleaseCount
            ,providers.email
            ,providers.phone
            ,providers.website
            ,providers.leavers
            ,providers.achievementRate
            ,providers.employerReviews
            ,providers.employerStars
            ,providers.employerRating
            ,providers.apprenticeReviews
            ,providers.apprenticeStars
            ,providers.apprenticeRating
        FROM (SELECT @userid "userId", @QARPeriod "qarPeriod", @ReviewPeriod "reviewPeriod", MAX(CreatedDate) maxCreatedDate
              FROM #MainQuery) toplevel
        JOIN (
            SELECT DISTINCT @userid "userId", "ordering", larsCode,standardName 
            FROM #MainQuery ) AS courses 
        ON courses."userId" = toplevel."userId"
        JOIN (
            SELECT DISTINCT larsCode, "l2.ordering" ordering, locationDescription 
            FROM #MainQuery ) AS locations 
        ON courses.larsCode = locations.larsCode
        JOIN (
            SELECT larsCode, "l2.ordering" 
            ,"l2.p3.ordering" ordering
            ,shortlistId shortlistId
            ,MainQuery.ukprn ukprn
            ,providerName providerName
            ,CAST(CASE WHEN AtEmployer = 1 THEN 1 ELSE 0 END AS BIT) atEmployer
            ,CAST(CASE WHEN BlockRelease = 1 THEN 1 ELSE 0 END AS BIT) hasBlockRelease
            ,ISNULL(BlockReleaseCount,0) blockReleaseCount                                             
            ,CASE WHEN LocationDescription IS NULL 
                  THEN null
                  ELSE CONVERT(DECIMAL(6,1),BlockReleaseDistance) END blockReleaseDistance
            ,CAST(CASE WHEN DayRelease = 1 THEN 1 ELSE 0 END AS BIT)  hasDayRelease
            ,CASE WHEN LocationDescription IS NULL 
                  THEN null
                  ELSE CONVERT(DECIMAL(6,1),DayReleaseDistance) END dayReleaseDistance
            ,ISNULL(DayReleaseCount,0) dayReleaseCount                                          
            ,email email
            ,phone phone
            ,website website
            -- Achievement Rates
            ,ISNULL(qp1.[Leavers],'-') leavers
            ,ISNULL(qp1.[AchievementRate],'-') achievementRate
            -- Star Ratings
            ,ISNULL(CONVERT(varchar,pes.ReviewCount),'None') employerReviews
            ,ISNULL(CONVERT(varchar,pes.Stars),'-') employerStars
            ,ISNULL(pes.Rating,'NotYetReviewed') employerRating
            ,ISNULL(CONVERT(varchar,pas.ReviewCount),'None') apprenticeReviews
            ,ISNULL(CONVERT(varchar,pas.Stars),'-') apprenticeStars
            ,ISNULL(pas.Rating,'NotYetReviewed') apprenticeRating

            FROM #MainQuery MainQuery 
            LEFT JOIN ProviderQARs qp1 on qp1.[Ukprn] = MainQuery.ukprn AND qp1.[IfateReferenceNumber] = MainQuery.[IfateReferenceNumber]
            LEFT JOIN EmployerStars pes on pes.[Ukprn] = MainQuery.ukprn 
            LEFT JOIN ApprenticeStars pas on pas.[Ukprn] = MainQuery.ukprn
        ) AS providers
        ON locations.larsCode = providers.larsCode AND locations.ordering = providers."l2.ordering"
        ORDER BY courses.ordering, locations.ordering, providers.ordering
        FOR JSON AUTO, INCLUDE_NULL_VALUES, WITHOUT_ARRAY_WRAPPER
    );
    
    DROP TABLE #MainQuery;
    SELECT REPLACE(@JSON,'\/','/');
END
