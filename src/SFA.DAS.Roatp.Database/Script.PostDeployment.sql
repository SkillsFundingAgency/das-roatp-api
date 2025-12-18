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
IF Exists(SELECT * from sys.indexes where name = 'UK_ProviderLocation_ProviderId_LocationName')
ALTER TABLE [dbo].[ProviderLocation] DROP CONSTRAINT [UK_ProviderLocation_ProviderId_LocationName]
GO

:r .\PostDeploymentScripts\PopulateRegionData.sql
:r .\PostDeploymentScripts\CSP-2360_Insert_Short_Course_test_data_for_course_management.sql