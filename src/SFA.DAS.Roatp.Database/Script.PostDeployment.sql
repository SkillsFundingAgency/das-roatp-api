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

IF OBJECT_ID(N'[dbo].[RestrictedProviderView]', N'V') IS NOT NULL
BEGIN
    DROP VIEW [dbo].[RestrictedProviderView];
END
GO

IF OBJECT_ID(N'[dbo].[RestrictedProviderCourseType]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[RestrictedProviderCourseType];
END
GO

IF Exists(SELECT * from sys.indexes where name = 'UK_ProviderLocation_ProviderId_LocationName')
ALTER TABLE [dbo].[ProviderLocation] DROP CONSTRAINT [UK_ProviderLocation_ProviderId_LocationName]
GO

:r .\PostDeploymentScripts\PopulateRegionData.sql
:r .\PostDeploymentScripts\CSP-2710_Database_updates_for_Provider_Restrictions.sql

