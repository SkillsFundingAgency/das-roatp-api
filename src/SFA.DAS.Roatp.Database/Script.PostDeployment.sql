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
-- preset QAR and Feedback
:r .\PostDeploymentScripts\CSP-1831-Insert_NationalQAR.sql
:r .\PostDeploymentScripts\CSP-1831-Insert_ProviderQAR.sql
:r .\PostDeploymentScripts\CSP-1831-Insert_StandardProviderQAR.sql
:r .\PostDeploymentScripts\CSP-1831-Insert_StandardQAR.sql
:r .\PostDeploymentScripts\CSP-1832-Insert_ProviderApprenticeStars.sql
:r .\PostDeploymentScripts\CSP-1832-Insert_ProviderEmployerStars.sql
