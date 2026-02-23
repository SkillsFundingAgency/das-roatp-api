-- this calculates the number of Training Providers
-- and the count of providers available for a location 
-- for a list of lars codes

CREATE PROCEDURE [dbo].[GetTrainingProvidersCount]
    @Latitude FLOAT NULL,
    @Longitude FLOAT NULL,
    @Distance INT NULL,
    @LarsCodes VARCHAR(2000)
AS
BEGIN
   SET NOCOUNT ON;

    DECLARE @NearestRegionId INT, 
            @AlternativeRegionId INT,
            @National INT = 1,
            @Provider INT = 0,
            @Regional INT = 2,
            @LarsCodeJSON VARCHAR(4000),
            @EPSG_COORDINATE_SYSTEM_ID INT = 4326,
            @MetersToMilesMetric FLOAT = 0.0006213712;
   

    IF @Latitude IS NOT NULL 
    -- match to nearest region (which may have an alternative with same co-ordinates)
        SELECT TOP 1 @NearestRegionId = reg1.[Id] , @AlternativeRegionid = reg2.[id]
        FROM [dbo].[Region] reg1
        LEFT JOIN [dbo].[Region] reg2 ON reg1.[Latitude] = reg2.[Latitude] AND reg1.[Longitude]= reg2.[Longitude] AND reg1.[Id] != reg2.[Id]
            ORDER BY geography::Point(reg1.Latitude, reg1.Longitude, @EPSG_COORDINATE_SYSTEM_ID)
                    .STDistance(geography::Point(@Latitude, @Longitude, @EPSG_COORDINATE_SYSTEM_ID)), reg1.[id];
    ELSE
        SET @Distance = NULL;

    -- ensure that the Lars code(s) input are a list of string values
    SET @LarsCodeJSON = '["'+REPLACE(REPLACE(@larscodes,'"',''),',','","')+'"]';
    -- get the Standards and national QAR by Standard
    WITH
    StandardsList
    AS
    (
        SELECT CONVERT(int,[key]) +1 Ordering, std.LarsCode , std.IsRegulatedForProvider, std.CourseType
        FROM OPENJSON(@LarsCodeJSON,'$') js1         
        JOIN [dbo].[Standard] std on std.LarsCode = [value]
    )

    -- Main Query
    SELECT 
        -- order by given order
         st1.[LarsCode] as 'LarsCode'
        ,ISNULL(ActiveProviders,0) AS 'ProvidersCount'
        ,ISNULL(AllProviders,0) AS 'TotalProvidersCount'
        
    FROM StandardsList st1
    LEFT JOIN (

        -- Course Management Courses
        SELECT 
                  
             Larscode
            ,COUNT(DISTINCT(CASE 
                            WHEN @Distance IS NULL
                            THEN Ukprn
                            WHEN LocationOrdering != 9 -- exclude outside Regions
                             AND Distance <= @Distance
                            THEN Ukprn
                            ELSE NULL END) ) ActiveProviders 
           ,COUNT(DISTINCT Ukprn) AllProviders
                                              
        FROM
            (
            -- Course Management Location data
            SELECT pr1.[Ukprn]
                    ,pc1.[LarsCode]
                    ,CASE
                     WHEN @Latitude  IS NULL THEN 0  -- No Location check
                     WHEN [LocationType] = 0 THEN 2  -- Provider
                     WHEN [LocationType] = 1 THEN 1  -- National
                     WHEN pl1.[RegionId] IS NOT NULL THEN 
                        (CASE WHEN pl1.[RegionId] = @NearestRegionId THEN 0 -- same Region
                              WHEN @AlternativeRegionid IS NOT NULL AND pl1.[RegionId] = @AlternativeRegionid THEN 0 -- alternative Region
                              ELSE 9 -- other Regions
                              END)
                     ELSE 9 -- other
                     END LocationOrdering
                    -- calculate distance 
                    ,CASE 
                    WHEN @Latitude IS NOT NULL AND [LocationType] = @Provider
                        THEN ROUND(geography::Point(pl1.Latitude, pl1.Longitude, @EPSG_COORDINATE_SYSTEM_ID)
                                    .STDistance(geography::Point(convert(float,@Latitude), convert(float,@Longitude), @EPSG_COORDINATE_SYSTEM_ID)) * @MetersToMilesMetric,1) 
                    ELSE 0 END AS Distance
            FROM [dbo].[ProviderCourse] pc1 
                JOIN StandardsList lc1 on lc1.LarsCode = pc1.LarsCode
                JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId
                JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
                -- ensure course type is (still) available for the provider and course
                JOIN [dbo].[ProviderCourseType] pct on pct.Ukprn = pr1.[Ukprn] AND pct.CourseType = lc1.CourseType
                JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
                JOIN [dbo].[ProviderLocation] pl1 on pl1.Id = pcl1.ProviderLocationId
                LEFT JOIN [dbo].[Region] rg1 on rg1.[Id] = pl1.[RegionId]
                --regulated check
                WHERE (lc1.IsRegulatedForProvider = 0 OR (lc1.IsRegulatedForProvider = 1 AND IsNull(IsApprovedByRegulator, 0) = 1))

            UNION ALL
            -- Course Management Online courses data
            SELECT pr1.[Ukprn]
                  ,pc1.[LarsCode]
                  ,-1 LocationOrdering
                  ,0 Distance
            FROM [dbo].[ProviderCourse] pc1 
                JOIN StandardsList lc1 on lc1.LarsCode = pc1.LarsCode
                JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId
                JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
                -- ensure course type is (still) available for the provider and course
                JOIN [dbo].[ProviderCourseType] pct on pct.Ukprn = pr1.[Ukprn] AND pct.CourseType = lc1.CourseType
                --regulated check
                WHERE (lc1.IsRegulatedForProvider = 0 OR (lc1.IsRegulatedForProvider = 1 AND IsNull(IsApprovedByRegulator, 0) = 1))
                -- online check
                AND ISNULL(pc1.HasOnlineDeliveryOption,0) = 1
            ) ab1 
        WHERE LocationOrdering != 9  -- exclude outside Regions
        GROUP BY Larscode
        ) ab2
    ON st1.LarsCode = ab2.LarsCode
    ORDER BY Ordering;
END
