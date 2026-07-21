/*
Pre-Deployment Script Template
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.
--------------------------------------------------------------------------------------
*/

-- Rename Shortlist.LocationDescription to LocationName before schema compare,
-- so existing data is preserved (avoids drop/add column).
IF COL_LENGTH('dbo.Shortlist', 'LocationDescription') IS NOT NULL
   AND COL_LENGTH('dbo.Shortlist', 'LocationName') IS NULL
BEGIN
    EXEC sp_rename N'dbo.Shortlist.LocationDescription', N'LocationName', N'COLUMN';
END
GO
