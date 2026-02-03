CREATE PROCEDURE [dbo].[GetCourseProviderDetails]
	@Ukprn BIGINT,
	@larscode nvarchar(10),
	@Latitude FLOAT NULL,
	@Longitude FLOAT NULL,
	@Location VARCHAR(200) NULL,
	@UserId UNIQUEIDENTIFIER
AS
BEGIN

	SET NOCOUNT ON;

	-- This get details for Training Provider training locations / regions, with calculates the distance is location provided 

	-- the latest QAR data time period and Feedback reviews
	DECLARE @QARPeriod varchar(4), @ReviewPeriod varchar(6), @feedbackperiod varchar(10);

	-- the QAR Period is the last one for which we have data
	SELECT TOP 1 @QARPeriod = [TimePeriod] FROM [dbo].[NationalQAR] ORDER BY [TimePeriod] DESC;

	-- The Employer and Apprentice Review period is the previous AY
	SELECT @ReviewPeriod = RIGHT(YEAR(DATEADD(month,-19,getutcdate())),2)+RIGHT(YEAR(DATEADD(month,-7,getutcdate())),2);
	
	-- For Employer and Apprentice feedback time period is "All" or "AYxxxx"
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
			@AlternativeRegionid int,
			@NearestRegion VARCHAR(200);
	IF @Latitude IS NOT NULL 

	-- match to nearest region (which may have an alternative with same co-ordinates)
		SELECT TOP 1 @NearestRegionId = reg1.[Id] , @NearestRegion = reg1.SubregionName + ' ('+reg1.RegionName+')' , @AlternativeRegionid = reg2.[id]
		FROM [dbo].[Region] reg1
		LEFT JOIN [dbo].[Region] reg2 ON reg1.[Latitude] = reg2.[Latitude] AND reg1.[Longitude]= reg2.[Longitude] AND reg1.[Id] != reg2.[id]
			ORDER BY geography::Point(reg1.Latitude, reg1.Longitude, 4326)
					.STDistance(geography::Point(@Latitude, @Longitude, 4326)), reg1.[id];
	ELSE
	-- cannot have longitude with no latitude
		SET @Longitude = NULL;
	-- the Standards and national QAR by Standard
	WITH StandardsAndQAR AS
	(
		SELECT Larscode, Title, [Level], [IsRegulatedForProvider], st1.[IfateReferenceNumber], ISNULL(qar1.[Leavers],'-') [Leavers], ISNULL(qar1.[AchievementRate],'-') [AchievementRate]
		FROM (
			SELECT [LarsCode], [IfateReferenceNumber], [Title], [Level], [IsRegulatedForProvider]
			FROM [dbo].[Standard] 
			WHERE [LarsCode] = @larscode
		) st1
		LEFT JOIN (
			SELECT [IfateReferenceNumber], [Leavers], [AchievementRate]
			FROM [dbo].[StandardQAR] 
			WHERE [TimePeriod] = @QARPeriod
		) qar1 on qar1.[IfateReferenceNumber] = st1.[IfateReferenceNumber]
	),

	-- the Standards and Provider QAR by Standard
	ProviderQARs AS
	(
		SELECT [Ukprn], [IfateReferenceNumber], [Leavers], [AchievementRate]
		FROM [dbo].[StandardProviderQAR] 
		WHERE [TimePeriod] = @QARPeriod
		AND [Ukprn] = @ukprn
	),

	-- The Employer feedback
	EmployerStars AS
	(
		SELECT [Ukprn], [ReviewCount], [Stars], [Rating]
		FROM [dbo].[ProviderEmployerStars] 
		WHERE [TimePeriod] = @feedbackperiod
		AND [Ukprn] = @ukprn
	),

	-- The Apprentice feedback
	ApprenticeStars AS
	(
		SELECT [Ukprn], [ReviewCount], [Stars], [Rating]
		FROM [dbo].[ProviderApprenticeStars] 
		WHERE TimePeriod = @feedbackperiod
		AND [Ukprn] = @ukprn
	)

	-- Main query
	SELECT 
		 ab2.Ukprn AS 'Ukprn'
		,ab2.LegalName AS 'ProviderName'
		,CAST(MAX(CASE WHEN ISNULL(ab2.HasOnlineDeliveryOption,0)=1 THEN 1 ELSE 0 END) 
			  OVER (PARTITION BY ab2.Larscode, ab2.Ukprn) AS bit) HasOnlineDeliveryOption
		,ab2.CourseType AS 'CourseType'
		,MainAddressLine1 AS 'MainAddressLine1'
		,MainAddressLine2 AS 'MainAddressLine2'
		,MainAddressLine3 AS 'MainAddressLine3'
		,MainAddressLine4 AS 'MainAddressLine4'
		,MainTown AS 'MainTown'
		,MainPostcode AS 'MainPostcode'
		,MarketingInfo AS 'MarketingInfo'
		,ContactUsEmail AS 'Email'
		,ContactUsPhoneNumber AS 'PhoneNumber'
		,ContactUsPageUrl AS 'WebSite'
		,stq.[Title] AS 'CourseName'
		,stq.[Level] AS 'Level'
		,ab2.Larscode AS 'LarsCode'
		,stq.IfateReferenceNumber AS 'IFateReferenceNumber'

		-- Achievement Rates
		,@QARPeriod AS 'Period'
		,ISNULL(qp1.[Leavers],'-') AS 'Leavers'
		,ISNULL(qp1.[AchievementRate],'-') AS 'AchievementRate'
		,ISNULL(stq.[Leavers],'-')  AS 'NationalLeavers'
		,ISNULL(stq.[AchievementRate],'-')  AS 'NationalAchievementRate'

		-- Reviews
		,@ReviewPeriod AS 'ReviewPeriod'
		,ISNULL(CONVERT(varchar,pes.ReviewCount),'None') AS 'EmployerReviews'
		,ISNULL(CONVERT(varchar,pes.Stars),'-') AS 'EmployerStars'
		,ISNULL(CONVERT(varchar(20),pes.Rating),'NotYetReviewed')  AS 'EmployerRating'
		,ISNULL(CONVERT(varchar,pas.ReviewCount),'None')  AS 'ApprenticeReviews'
		,ISNULL(CONVERT(varchar,pas.Stars),'-') AS 'ApprenticeStars'
		,ISNULL(CONVERT(varchar(20),pas.Rating),'NotYetReviewed') AS 'ApprenticeRating'

		-- Locations types
		,CONVERT(BIT, AtEmployer) AS 'AtEmployer'
		,CONVERT(BIT, BlockRelease) AS 'BlockRelease'
		,CONVERT(BIT, DayRelease) AS 'DayRelease'

		-- Ordering calculation
		 ,ROW_NUMBER() OVER (PARTITION BY ab2.Larscode, ab2.ukprn ORDER BY LocationOrdering, Course_Distance, Course_Location) AS 'Ordering'

		-- List of locations
		,LocationType AS 'LocationType'
		,Course_Location AS 'CourseLocation'
		,AddressLine1 AS 'AddressLine1'
		,AddressLine2 AS 'AddressLine2'
		,Town AS 'Town'
		,County AS 'County'
		,Postcode AS 'Postcode'
		,Course_Distance AS 'CourseDistance'
		,sht.[Id] AS 'ShortlistId'
	FROM 
	(
		SELECT 
			 Ukprn, 
			 LegalName
			,Larscode
			,LocationType
			,AtEmployer
			,BlockRelease
			,DayRelease
			,HasOnlineDeliveryOption
			,CourseType
			,Course_Location
			,LocationOrdering
			,Distance
			,LocationName
			,AddressLine1
			,AddressLine2
			,Town
			,County
			,Postcode
			,MainAddressLine1
			,MainAddressLine2
			,MainAddressLine3
			,MainAddressLine4
			,MainTown
			,MainPostcode
			,MarketingInfo
			,ContactUsEmail
			,ContactUsPhoneNumber
			,ContactUsPageUrl
			-- LocationType: Provider = 0, National = 1, Regional = 2
			,CASE WHEN LocationType = 0 THEN Distance
				  WHEN LocationOrdering = 3 THEN 0
				  ELSE 0 END Course_Distance
			-- priority for at workplace over at provider (by ukprn and course)
			,ROW_NUMBER() OVER (PARTITION BY [Ukprn], [LarsCode], 
								CASE WHEN LocationType = 0 THEN 1 ELSE 0 END 
								ORDER BY Distance) TP_Std_Dist_Seq
			,MIN(LocationOrdering) OVER (PARTITION BY [Ukprn], [LarsCode]) Min_LocationOrdering		
			,IsApprovedByRegulator
		FROM
			(
			-- Managing Standards Course Location data
			SELECT pr1.[Ukprn], pr1.LegalName
				  ,pc1.[LarsCode]
				  ,[LocationType]
				  -- Is at Employer ?
				  ,CASE [LocationType] 
				   WHEN 0 THEN 0 -- At provider
				   WHEN 2 THEN -- Regional
					(CASE WHEN @Latitude IS NULL
						  THEN 1
						  WHEN pl1.[RegionId] IS NOT NULL AND pl1.[RegionId] = @NearestRegionId 
						  THEN 1 -- same Region
						  WHEN pl1.[RegionId] IS NOT NULL AND @AlternativeRegionid IS NOT NULL AND pl1.[RegionId] = @AlternativeRegionid THEN 1 -- alternative Region
						  ELSE 0 -- other Regions are outside
						  END)
				   ELSE 1 END AtEmployer
				  ,ISNULL(HasBlockReleaseDeliveryOption,0) BlockRelease
				  ,ISNULL(HasDayReleaseDeliveryOption,0) DayRelease
				  ,ISNULL(pc1.[HasOnlineDeliveryOption],0) HasOnlineDeliveryOption
				  ,s1.[CourseType]
				  ,CASE [LocationType] 
				   WHEN 0 THEN pl1.Postcode
				   WHEN 1 THEN 'National'
				   WHEN 2 THEN SubregionName + ' ('+RegionName+')'
				   END Course_Location
				  ,CASE
				   WHEN @Latitude IS NULL THEN 0  -- no distance check
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
				   ,pl1.LocationName
				   ,pl1.AddressLine1
				   ,pl1.AddressLine2
				   ,pl1.Town
				   ,pl1.County
				   ,pl1.Postcode
				   ,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine1 ELSE tp.AddressLine1 END MainAddressLine1
				   ,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine2 ELSE tp.AddressLine2 END MainAddressLine2
				   ,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine3 ELSE tp.AddressLine3 END MainAddressLine3
				   ,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine4 ELSE tp.AddressLine4 END MainAddressLine4
				   ,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.Town ELSE tp.Town END MainTown
				   ,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.Postcode ELSE tp.Postcode END MainPostcode
				   ,pr1.MarketingInfo
				   ,ISNULL(pc1.ContactUsEmail,pr1.Email) ContactUsEmail
				   ,ISNULL(pc1.ContactUsPhoneNumber, pr1.Phone) ContactUsPhoneNumber
				   ,ISNULL(pc1.StandardInfoUrl, pr1.Website) ContactUsPageUrl
				   ,pc1.[IsApprovedByRegulator]
			  FROM [dbo].[ProviderCourse] pc1 
			  JOIN [dbo].[Provider] pr1 on pr1.Id = pc1.ProviderId
			  JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] AND tp.[Statusid] = 1 AND tp.[ProviderTypeId] = 1 -- Active, Main only
			  LEFT JOIN [dbo].[ProviderAddress] pad on pad.ProviderId = pr1.Id
			  JOIN [dbo].[ProviderCourseLocation] pcl1 on pcl1.ProviderCourseId = pc1.[Id]
			  JOIN [dbo].[ProviderLocation] pl1 on pl1.Id = pcl1.ProviderLocationId
			  JOIN [dbo].[Standard] s1 on s1.LarsCode = pc1.LarsCode
			  JOIN [dbo].[ProviderCourseType] pct1 on pct1.Ukprn = pr1.Ukprn AND pct1.CourseType = s1.CourseType
			  LEFT JOIN [dbo].[Region] rg1 on rg1.[Id] = pl1.[RegionId]
			  WHERE 
				  -- specific Training Course 
				  pc1.[LarsCode] = @larscode
				  AND pr1.[Ukprn] = @ukprn
			  ) ab1 
		) ab2
	-- Standards and QAR data
	JOIN StandardsAndQAR stq on stq.LarsCode = ab2.LarsCode
	LEFT JOIN ProviderQARs qp1 on qp1.[Ukprn] = ab2.[Ukprn] AND qp1.[IfateReferenceNumber] = stq.[IfateReferenceNumber]
	LEFT JOIN EmployerStars pes on pes.[Ukprn] = ab2.[Ukprn] 
	LEFT JOIN ApprenticeStars pas on pas.[Ukprn] = ab2.[Ukprn] 
	LEFT JOIN [dbo].[Shortlist] sht on sht.[Ukprn] = ab2.[Ukprn] AND sht.[Larscode] = ab2.LarsCode AND sht.[userId] = @userId
		AND ((sht.[LocationDescription] IS NULL AND @Location IS NULL) OR (sht.[LocationDescription] = @Location))
	-- regulated check
	WHERE stq.IsRegulatedForProvider = 0 OR (stq.IsRegulatedForProvider = 1 AND IsApprovedByRegulator = 1)
	ORDER BY ukprn, Larscode, Ordering
	
END