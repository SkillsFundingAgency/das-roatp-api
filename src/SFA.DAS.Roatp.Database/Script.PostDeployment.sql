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
IF Exists(SELECT * from sys.indexes where name = 'UK_ProviderLocation_ProviderId_LocationName')
ALTER TABLE [dbo].[ProviderLocation] DROP CONSTRAINT [UK_ProviderLocation_ProviderId_LocationName]
GO

GO
IF Exists(SELECT * from sys.indexes where name = 'UK_ProviderLocation_ProviderId__LocationType_LocationName')
ALTER TABLE [dbo].[ProviderLocation] DROP CONSTRAINT [UK_ProviderLocation_ProviderId__LocationType_LocationName]
GO

CREATE UNIQUE NONCLUSTERED INDEX UK_ProviderLocation_ProviderId__LocationType_LocationName
ON ProviderLocation(ProviderId, LocationType,LocationName)
WHERE LocationName IS NOT NULL;

:r .\PostDeploymentScripts\PopulateRegionData.sql
