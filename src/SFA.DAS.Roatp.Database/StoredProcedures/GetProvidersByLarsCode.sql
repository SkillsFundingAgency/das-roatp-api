﻿CREATE procedure [dbo].[GetProvidersByLarsCode]
	@larscode int,   -- Standard by LarsCode - must be set
	@SortOrder varchar(30) = 'Distance', -- order by "Distance", "AchievementRate" or "EmployerProviderRating" , "ApprenticeProviderRating"
	@page int = 1,
	@pageSize int = 10,
		-- FILTERS
	@workplace bit = null, -- 0 or 1 include training at apprentice's workplace
	@provider bit = null,     -- 0 or 1 include training at providers+
	@blockrelease bit = null, -- 0 or 1 include block release
	@dayrelease bit = null,   -- 0 or 1 include day release
	@Latitude float = null,
	@Longitude float = null,
	@Distance int = null, -- Distance should always set when Longitude & Longitude set
	@QARrange varchar(100) = null, -- any combo of 'excellent', 'good', 'poor', 'verypoor' and 'none' , or NULL
	@employerProviderRatings varchar(100) = null, -- any combo of 'excellent', 'good', 'poor', 'verypoor' and 'notyetreviewed' , or NULL
	@apprenticeProviderRatings varchar(100) = null -- any combo of 'excellent', 'good', 'poor', 'verypoor' and 'notyetreviewed' , or NULL
as

SET NOCOUNT ON
-- this calculates the distance from Training Provider training locations / regions with filters
--DECLARE 
--@page int = 1,
--@pageSize int = 10,
---- Location can be Null or based on postcode lookup e.g. PRESTION PR1 2ED (52.57339, -0.24848)
--@Latitude float,-- = 52.57339,  
--@Longitude float,-- = -0.24848, 
-- Distance should always set when Longitude & Longitude set
--@Distance int,-- = 30,     -- Distance optional
-- Filters
--@workplace int = 1,    -- 0 or 1 include training at apprentice's workplace
--@provider int = 1,     -- 0 or 1 include training at providers+
--@blockrelease int = 1, -- 0 or 1 include block release
--@dayrelease int = 1,   -- 0 or 1 include day release
-- Standard by LarsCode - must be set
--@larscode int = 1,
-- any combo of 'excellent', 'good', 'poor', 'verypoor' and 'none' , or NULL
--@QARrange varchar(100),
-- any combo of 'excellent', 'good', 'poor', 'verypoor' and 'notyetreviewed' , or NULL
--@employerProviderRatings varchar(100),
-- any combo of 'excellent', 'good', 'poor', 'verypoor' and 'notyetreviewed' , or NULL
--@apprenticeProviderRatings varchar(100),
-- order by "Distance", "AchievementRate" or "EmployerProviderRating" , "ApprenticeProviderRating"
--@SortOrder varchar(30) = 'Distance';  

-- local working
DECLARE @skip int = (@page - 1) * @pageSize;

-- the latest QAR data time period and Feedback reviews
DECLARE @QARPeriod varchar(4), @ReviewPeriod varchar(6), @feedbackperiod varchar(10)

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

-- for nearest region name and match by regionid
DECLARE @NearestRegionId int, 
        @AlternativeRegionid int;

IF @Latitude IS NOT NULL 
-- match to nearest region (which may have an alternative with same co-ordinates)
    SELECT TOP 1 @NearestRegionId = reg1.[Id] , @AlternativeRegionid = reg2.[id]
    FROM [dbo].[Region] reg1
    LEFT JOIN [dbo].[Region] reg2 ON reg1.[Latitude] = reg2.[Latitude] AND reg1.[Longitude]= reg2.[Longitude] AND reg1.[Id] != reg2.[id]
        ORDER BY geography::Point(reg1.Latitude, reg1.Longitude, 4326)
                .STDistance(geography::Point(@Latitude, @Longitude, 4326)), reg1.[id];
ELSE
-- cannot have distance with no co-ordinates
    SET @Distance = NULL;


-- the Standards and national QAR by Standard
WITH Standards
AS
(
    SELECT [LarsCode], [IfateReferenceNumber], [Title], [Level] 
    FROM [dbo].[Standard] 
    WHERE [LarsCode] = @larscode
)
,
-- the Standards and Provider QAR by Standard
ProviderQARs
AS
(
    SELECT [Ukprn], qar.[IfateReferenceNumber], [Leavers], [AchievementRate]
    ,CASE WHEN ISNULL([AchievementRate],'x') = 'x' 
          THEN 'None'
          WHEN [AchievementRate] < '50' THEN 'VeryPoor'
          WHEN [AchievementRate] < '60' THEN 'Poor'
          WHEN [AchievementRate] < '70' THEN 'Good'
          ELSE 'Excellent' END AchievementRank
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
),
-- Results query
Results
AS
(
    SELECT 
        ab2.Larscode 
        ,COUNT(*) OVER (PARTITION BY ab2.Larscode) totalcount
         -- Ordering calculation
        ,ROW_NUMBER() OVER (PARTITION BY ab2.Larscode 
                            ORDER BY 
                            -- Distance
                             CASE WHEN @SortOrder = 'Distance' THEN MIN(LocationOrdering) ELSE 1 END
                            ,CASE WHEN @SortOrder = 'Distance' THEN MIN(Course_Distance) ELSE 1 END
                            -- Achievement Rate
                            ,CASE WHEN @SortOrder = 'AchievementRate' THEN
                                 (CASE WHEN qp1.AchievementRate IS NULL THEN -1 
                                       WHEN qp1.AchievementRate = 'x' THEN 0 
                                       ELSE CONVERT(float,qp1.AchievementRate)
                                       END) 
                                  ELSE 1 END DESC
                            -- Employer Star Rating
                            ,CASE WHEN @SortOrder = 'EmployerProviderRating' THEN ISNULL(pes.Stars,-1) ELSE 1 END DESC 
                            -- Apprentice Star Rating 
                            ,CASE WHEN @SortOrder = 'ApprenticeProviderRating' THEN ISNULL(pas.Stars,-1) ELSE 1 END DESC
                            -- and then always by Distance
                            ,MIN(LocationOrdering)
                            ,MIN(Course_Distance)
                            -- and then always by Achevement Rate
                            ,CASE WHEN qp1.AchievementRate IS NULL THEN -1 
                                  WHEN qp1.AchievementRate = 'x' THEN 0 
                                  ELSE CONVERT(float,qp1.AchievementRate) END DESC
                            -- and then always by Employer and Apprentice Provider Ratings
                            ,ISNULL(pes.Stars,-1) DESC         -- Employer Star Rating
                            ,ISNULL(pas.Stars,-1) DESC         -- Apprentice Star Rating 
                            -- and all being equal nearest provider location (if any)
                            ,MIN(CASE WHEN Course_Distance = 0 THEN 99999 ELSE Course_Distance END)
                            -- and finally by the UKPRN to enforce same ordering
                            ,ab2.Ukprn ) - (@pageSize * (@page-1)) "providers.ordering"  -- and ordered within the page of results
        ,ab2.Ukprn "providers.ukprn"
        ,ab2.LegalName "providers.providername"
        -- List of locations
        ,COUNT(*) "providers.locationsCount"
        ,STRING_AGG(LocationType,',') WITHIN GROUP (ORDER BY LocationOrdering, Distance) "providers.locations.locationType"
        ,STRING_AGG(CONVERT(varchar,Course_Distance),', ') WITHIN GROUP (ORDER BY LocationOrdering, Distance) "providers.locations.courseDistances"
        ,STRING_AGG(CONVERT(varchar,AtEmployer),',') WITHIN GROUP (ORDER BY LocationOrdering, Distance) "providers.locations.AtEmployer"
        ,STRING_AGG(CONVERT(varchar,DayRelease),',') WITHIN GROUP (ORDER BY LocationOrdering, Distance) "providers.locations.DayRelease"
        ,STRING_AGG(CONVERT(varchar,BlockRelease),',') WITHIN GROUP (ORDER BY LocationOrdering, Distance) "providers.locations.BlockRelease"
        -- Achievement Rates
        ,ISNULL(qp1.[Leavers],'-') "providers.leavers"
        ,ISNULL(qp1.[AchievementRate],'-') "providers.achievementRate"
        -- Star Ratings
        ,ISNULL(CONVERT(varchar,pes.ReviewCount),'None') "providers.employerReviews"
        ,ISNULL(CONVERT(varchar,pes.Stars),'-') "providers.employerStars"
        ,ISNULL(pes.Rating,'NotYetReviewed') "providers.employerRating"
        ,ISNULL(CONVERT(varchar,pas.ReviewCount),'None') "providers.apprenticeReviews"
        ,ISNULL(CONVERT(varchar,pas.Stars),'-') "providers.apprenticeStars"
        ,ISNULL(pas.Rating,'NotYetReviewed') "providers.apprenticeRating"
    FROM 
        (
        SELECT Ukprn, LegalName
            ,Larscode
            ,LocationType
            ,AtEmployer
            ,BlockRelease
            ,DayRelease
            ,Course_Location
            ,LocationOrdering
            ,Distance
            -- LocationType: Provider = 0, National = 1, Regional = 2
            ,CASE WHEN LocationType = 0 THEN Distance
                  WHEN LocationOrdering = 3 THEN 99999
                  ELSE 0 END Course_Distance
            -- priority for at workplace over at provider (by ukprn and course)
            ,ROW_NUMBER() OVER (PARTITION BY [Ukprn], [LarsCode], 
                                CASE WHEN LocationType = 0 THEN 1 ELSE 0 END 
                                ORDER BY Distance) TP_Std_Dist_Seq
        FROM
            (
            -- Managing Standards Course Location data
            SELECT pr1.[Ukprn], pr1.LegalName
                  ,[LarsCode]
                  ,[LocationType]
                  -- Is at Employer ?
                  ,CASE [LocationType] 
                   WHEN 0 THEN 0 ELSE 1 END AtEmployer
                  ,ISNULL(HasBlockReleaseDeliveryOption,0) BlockRelease
                  ,ISNULL(HasDayReleaseDeliveryOption,0) DayRelease
                  ,CASE [LocationType] 
                   WHEN 0 THEN pl1.Postcode
                   WHEN 1 THEN 'National'
                   WHEN 2 THEN SubregionName + ' ('+RegionName+')'
                   END Course_Location
                  ,CASE
                   WHEN @Latitude IS  NULL THEN 0  -- no distance check
                   WHEN [LocationType] = 0 THEN 2  -- Provider
                   WHEN [LocationType] = 1 THEN 1  -- National
                   WHEN pl1.[RegionId] IS NOT NULL THEN 
                        (CASE WHEN pl1.[RegionId] = @NearestRegionId THEN 0 -- same Region
                              WHEN @AlternativeRegionid IS NOT NULL AND pl1.[RegionId] = @AlternativeRegionid THEN 0 -- alternative Region
                              ELSE 3 -- other Regions
                              END)
                   ELSE 3 -- other
                   END LocationOrdering
                   -- calculate distance 
                  ,CASE 
                   WHEN @Latitude IS NOT NULL AND [LocationType] = 0  -- Provider
                   THEN ROUND(geography::Point(pl1.Latitude, pl1.Longitude, 4326)
                             .STDistance(geography::Point(convert(float,@Latitude), convert(float,@Longitude), 4326)) * 0.0006213712,1) 
                   WHEN [LocationType] = 1  -- National
                   THEN 0
                   WHEN @Latitude IS NOT NULL AND [LocationType] = 2  -- Regional
                   THEN ROUND(geography::Point(rg1.Latitude, rg1.Longitude, 4326)
                             .STDistance(geography::Point(convert(float,@Latitude), convert(float,@Longitude), 4326)) * 0.0006213712,1) 
                   ELSE 0 END AS Distance
              FROM [dbo].[ProviderCourse] pc1 
              JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId
              JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
              JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
              JOIN [dbo].[ProviderLocation] pl1 on pl1.Id = pcl1.ProviderLocationId
              LEFT JOIN [dbo].[Region] rg1 on rg1.[Id] = pl1.[RegionId]
              WHERE 1=1 
              -- specific Training Course 
              AND [LarsCode] = @larscode

              ) ab1 
        WHERE 1=1
        AND LocationOrdering != 3 -- exclude outside Regions
        -- Distance filter check if requested
        AND (@Distance IS NULL OR Distance <= @Distance)
        -- Logic to match to Checkboxes for training locations 
        -- At apprentice's workplace and/or training at providers
        -- And at training provider for Day release and/or Block Release options
        -- Provider = 0, National = 1, Regional = 2
        AND (CASE WHEN @workplace = 0 AND @provider = 0 
                  THEN 1 -- no filters
                  WHEN @workplace = 1 AND LocationType != 0 -- ('National','Regional') 
                  -- only allowed at workplace for national and regional
                  THEN 1
                  WHEN @workplace = 0 AND LocationType != 0 -- ('National','Regional') 
                  -- only allowed at workplace for national and regional
                  THEN 0
                  WHEN @provider = 0 AND LocationType = 0
                  THEN 0 
                  -- @provider = 1 and @workplace = 0 or 1
                  WHEN @provider = 1 
                  THEN (CASE 
                        WHEN @blockRelease = 0 AND @dayRelease = 0
                        THEN 1
                        WHEN @blockRelease = 1 AND @dayRelease = 1
                        -- include where either block or day release
                        THEN (CASE WHEN BlockRelease = 1 OR DayRelease = 1 THEN 1 ELSE 0 END)
                        WHEN @blockRelease = 1 
                        THEN (CASE WHEN BlockRelease = 1 THEN 1 ELSE 0 END)
                        WHEN @dayRelease = 1
                        THEN (CASE WHEN DayRelease = 1 THEN 1 ELSE 0 END)
                        ELSE 1 -- no filter on block/day release
                        END)
                  ELSE 1
             END) = 1
        ) ab2
    -- Standards and QAR data
    JOIN Standards stq on stq.LarsCode = ab2.LarsCode
    LEFT JOIN ProviderQARs qp1 on qp1.[Ukprn] = ab2.[Ukprn] AND qp1.[IfateReferenceNumber] = stq.[IfateReferenceNumber]
    LEFT JOIN EmployerStars pes on pes.[Ukprn] = ab2.[Ukprn] 
    LEFT JOIN ApprenticeStars pas on pas.[Ukprn] = ab2.[Ukprn] 
    WHERE 1=1
    -- this gets only one row for each UKPRN (by larscode) taking nearest location/region or National
    AND (LocationType = 0 OR TP_Std_Dist_Seq = 1)
    -- filter QAR
    AND (@QARrange IS NULL OR ','+@QARrange+',' LIKE '%,'+ISNULL(qp1.AchievementRank,'none')+',%' )
    -- filter Employer Reviews
    AND (@employerProviderRatings IS NULL OR ','+@employerProviderRatings+',' LIKE '%,'+ISNULL(pes.Rating,'NotYetReviewed')+',%' )
    --Filter Apprentice reviews
    AND (@apprenticeProviderRatings IS NULL OR ','+@apprenticeProviderRatings+',' LIKE '%,'+ISNULL(pas.Rating,'NotYetReviewed')+',%' )
    GROUP BY ab2.Ukprn ,ab2.LarsCode, ab2.LegalName, stq.Title, stq.[Level]
    , qp1.[Leavers], qp1.[AchievementRate]
    , pes.ReviewCount, pes.Stars, pes.Rating
    , pas.ReviewCount, pas.Stars, pas.Rating
    ORDER BY "providers.ordering"
    OFFSET @skip ROWS
    FETCH NEXT @pageSize ROWS ONLY
)
-- Main Query
SELECT  
     @page "page"
    ,@pageSize "pagesize"
    ,CONVERT(int,ROUND(((@pageSize/2.0-0.5)+isnull(totalcount,0))/@pagesize,0)) "totalpages"
    ,ISNULL(totalcount,0) totalcount
    ,Standards.Larscode larscode
    ,Standards.[Title]+' (level '+CONVERT(varchar,Standards.[Level])+')' standardName
    ,@QARPeriod qarPeriod
    ,@ReviewPeriod reviewPeriod
    ,"providers.ordering"
    ,"providers.ukprn"
    ,"providers.providername"
    ,"providers.locationsCount"
    ,"providers.locations.locationType"
    ,"providers.locations.courseDistances"
    ,"providers.locations.AtEmployer"
    ,"providers.locations.DayRelease"
    ,"providers.locations.BlockRelease"
    ,"providers.leavers"
    ,"providers.achievementRate"
    ,"providers.employerReviews"
    ,"providers.employerStars"
    ,"providers.employerRating"
    ,"providers.apprenticeReviews"
    ,"providers.apprenticeStars"
    ,"providers.apprenticeRating"
FROM Standards
LEFT JOIN Results on Results.Larscode = Standards.Larscode
;
