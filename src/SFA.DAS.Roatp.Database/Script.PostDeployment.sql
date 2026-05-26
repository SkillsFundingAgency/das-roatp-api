/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

GO

IF OBJECT_ID(N'[dbo].[NationalAchievementRateImport]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[NationalAchievementRateImport];
END
GO

IF OBJECT_ID(N'[dbo].[NationalAchievementRateOverallImport]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[NationalAchievementRateOverallImport];
END
GO

IF OBJECT_ID(N'[dbo].[NationalAchievementRateOverall]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[NationalAchievementRateOverall];
END
GO

IF OBJECT_ID(N'[dbo].[NationalAchievementRate]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[NationalAchievementRate];
END
GO

IF OBJECT_ID(N'[dbo].[ProviderCourseVersion]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[ProviderCourseVersion];
END
GO

IF Exists(SELECT * from sys.indexes where name = 'UK_ProviderLocation_ProviderId_LocationName')
ALTER TABLE [dbo].[ProviderLocation] DROP CONSTRAINT [UK_ProviderLocation_ProviderId_LocationName]
GO

:r .\PostDeploymentScripts\PopulateRegionData.sql
