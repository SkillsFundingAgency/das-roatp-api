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
CREATE UNIQUE NONCLUSTERED INDEX UK_ProviderLocation_ProviderId_LocationName
ON providerLocation([ProviderId], [LocationName])
WHERE LocationName IS NOT NULL
GO
CREATE UNIQUE NONCLUSTERED INDEX UK_ProviderLocation_ProviderId_RegionId
ON providerLocation([ProviderId], [RegionId])
WHERE RegionId IS NOT NULL
GO

GO
IF OBJECT_ID('[UK_ProviderLocation_ProviderId_LocationName]', 'UQ') IS NOT NULL 
ALTER TABLE [dbo].[ProviderLocation] DROP CONSTRAINT [UK_ProviderLocation_ProviderId_LocationName]
GO

:r .\PostDeploymentScripts\PopulateRegionData.sql
