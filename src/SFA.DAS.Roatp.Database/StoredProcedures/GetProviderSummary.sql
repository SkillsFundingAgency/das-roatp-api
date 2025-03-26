CREATE PROCEDURE [dbo].[GetProviderSummary]
    @Ukprn BIGINT NULL
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @PROVIDER_TYPE_MAIN INT = 1, 
	        @PROVIDER_TYPE_EMPLOYER INT = 2,
		@STATUS_ACTIVE INT = 1,
		@STATUS_ACTIVE_BUT_NOT_TAKING_ON_APPRENTICES INT = 2,
		@STATUS_ONBOARDING INT = 3

    -- This store procedure gets summary details for a individual training provider.

	-- Filters the latest QAR data time period and Feedback reviews
	DECLARE @QARPeriod varchar(4), 
	        @ReviewPeriod varchar(6), 
		@feedbackperiod varchar(10), 
		@Achievementrate varchar(6);

	SELECT @QARPeriod = MAX([TimePeriod]) FROM [dbo].[NationalQAR]

	SELECT @Achievementrate = [AchievementRate] FROM [dbo].[NationalQAR] WHERE [TimePeriod] = @QARPeriod;

	-- The Employer and Apprentice Review period is the previous AY

	SELECT @ReviewPeriod = RIGHT(YEAR(DATEADD(month,-19,getutcdate())),2)+RIGHT(YEAR(DATEADD(month,-7,getutcdate())),2);

	-- For Employer and Apprentice feedback time period is "All" or "AYxxxx"
	-- get latest full year we have data for - should be previous AY.

	SELECT @feedbackperiod = MAX([TimePeriod])
		FROM [dbo].[ProviderEmployerStars]
	WHERE [TimePeriod] LIKE 'AY%' AND [TimePeriod] <= 'AY' + @ReviewPeriod;

	IF @feedbackperiod IS NULL
		SET @feedbackperiod = 'AY' + @ReviewPeriod;
	ELSE
		SET @ReviewPeriod = REPLACE(@feedbackperiod, 'AY', '');

	-- PROVIDER QAR
	WITH ProviderQAR
	AS
	(
		SELECT [Ukprn], [Leavers], [AchievementRate]
			FROM [dbo].[ProviderQAR] 
		WHERE [TimePeriod] = @QARPeriod AND 
		      [Ukprn] = @ukprn
	),

	-- ENPLOYER FEEDBACK
	EmployerStars AS
	(
		SELECT [Ukprn], [ReviewCount], [Stars], [Rating]
			FROM [dbo].[ProviderEmployerStars] 
		WHERE [TimePeriod] = @feedbackperiod AND 
		      [Ukprn] = @ukprn
	),

	-- APPRENTICE FEEDBACK
	ApprenticeStars AS
	(
		SELECT [Ukprn], [ReviewCount], [Stars], [Rating]
		FROM [dbo].[ProviderApprenticeStars] 
			WHERE TimePeriod = @feedbackperiod AND 
			      [Ukprn] = @ukprn
	)

	-- MAIN QUERY
	SELECT 
		 ab1.Ukprn AS 'Ukprn'
		,ab1.LegalName AS 'LegalName'
		,ab1.TradingName AS 'TradingName'
		,Email AS 'Email'
		,Phone AS 'Phone'
		,Website AS 'ContactUrl'
		,ProviderTypeId AS 'ProviderTypeId'
		,StatusId AS 'StatusId'
		,CanAccessApprenticeshipService AS 'CanAccessApprenticeshipService'
		,MainAddressLine1 AS 'MainAddressLine1'
		,MainAddressLine2 AS 'MainAddressLine2'
		,MainAddressLine3 AS 'MainAddressLine3'
		,MainAddressLine4 AS 'MainAddressLine4'
		,MainTown AS 'MainTown'
		,MainPostcode AS 'MainPostcode'
		,MainLatitude AS 'Latitude'
		,MainLongitude AS 'Longitude'
		,MarketingInfo AS 'MarketingInfo'
		,@QARPeriod AS 'QARPeriod'
		,ISNULL(qp1.[Leavers], '-') AS 'Leavers'
		,ISNULL(qp1.[AchievementRate], '-') AS 'AchievementRate'
		,@AchievementRate AS 'NationalAchievementRate'
		,@ReviewPeriod AS 'ReviewPeriod'
		,ISNULL(CONVERT(varchar,pes.ReviewCount),'None') AS 'EmployerReviews'
		,ISNULL(CONVERT(varchar,pes.Stars), '-') AS 'EmployerStars'
		,ISNULL(pes.Rating,'NotYetReviewed') AS 'EmployerRating'
		,ISNULL(CONVERT(varchar,pas.ReviewCount),'None') AS 'ApprenticeReviews'
		,ISNULL(CONVERT(varchar,pas.Stars), '-') AS 'ApprenticeStars'
		,ISNULL(pas.Rating,'NotYetReviewed') AS 'ApprenticeRating'
	FROM 
		(
		-- Managing Standards Course Location data
		SELECT pr1.[Ukprn], pr1.LegalName, pr1.TradingName
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine1 ELSE tp.AddressLine1 END MainAddressLine1
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine2 ELSE tp.AddressLine2 END MainAddressLine2
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine3 ELSE tp.AddressLine3 END MainAddressLine3
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.AddressLine4 ELSE tp.AddressLine4 END MainAddressLine4
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.Town ELSE tp.Town END MainTown
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.Postcode ELSE tp.Postcode END MainPostcode
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.Latitude ELSE tp.Latitude END MainLatitude
				,CASE WHEN pad.ProviderId IS NOT NULL THEN pad.Longitude ELSE tp.Longitude END MainLongitude
				,pr1.MarketingInfo
				,pr1.Email 
				,pr1.Phone 
				,pr1.Website 
				,tp.[Statusid]
				,tp.[ProviderTypeId]
				,CASE WHEN tp.[ProviderTypeId] in (@PROVIDER_TYPE_MAIN, @PROVIDER_TYPE_EMPLOYER) AND 
				           tp.[StatusId] IN (@STATUS_ACTIVE, @STATUS_ACTIVE_BUT_NOT_TAKING_ON_APPRENTICES, @STATUS_ONBOARDING) 
					  THEN CONVERT(BIT, 1)
					  ELSE CONVERT(BIT, 0) END CanAccessApprenticeshipService
			FROM [dbo].[Provider] pr1 
			JOIN [dbo].[ProviderRegistrationDetail] tp on tp.[Ukprn] = pr1.[Ukprn] 
			LEFT JOIN [dbo].[ProviderAddress] pad on pad.ProviderId = pr1.Id
			WHERE pr1.[Ukprn] = @ukprn
		) ab1 
	LEFT JOIN ProviderQAR qp1 on qp1.[Ukprn] = ab1.[Ukprn] 
	LEFT JOIN EmployerStars pes on pes.[Ukprn] = ab1.[Ukprn] 
	LEFT JOIN ApprenticeStars pas on pas.[Ukprn] = ab1.[Ukprn] 
END