CREATE VIEW [dbo].[ForecastQuartersView]
AS
-- this calculates the next 4 quarters and AYs
WITH Quarters AS (
    SELECT 0 AS [QuarterPeriod]
    UNION ALL
    SELECT [QuarterPeriod]+3 AS [Quarter]
    FROM Quarters 
    WHERE [QuarterPeriod] < 9
)
SELECT 
    'AY'+RIGHT(YEAR(DATEADD(month,-7,QuarterDate)),2)+RIGHT(YEAR(DATEADD(month,5,QuarterDate)),2) [TimePeriod]
    ,(((15-((19-month(QuarterDate))%12))-1)/3) [Quarter]
    ,DATEADD(day,1,EOMONTH(QuarterDate,-((MONTH(QuarterDate)+10)%3)-1)) [StartDate]
    ,EOMONTH(QuarterDate,-((MONTH(QuarterDate)+10)%3)+2) [EndDate]
FROM (
    SELECT [QuarterPeriod], dateadd(month,[QuarterPeriod],GETUTCDATE()) QuarterDate
    FROM Quarters
) ab1;

GO
