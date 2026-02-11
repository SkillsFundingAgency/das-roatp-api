CREATE VIEW [dbo].[ProviderCoursesTimelineView]
AS 
  SELECT 
     ProviderId
    ,LarsCode
    ,CASE WHEN MIN(min_EffectiveFrom) IS NOT NULL
          THEN CONVERT(Date,MIN(min_EffectiveFrom)) 
          ELSE '2023-02-21' -- last import date
          END EffectiveFrom
    ,CASE WHEN MAX(max_EffectiveFrom) > MAX(max_EffectiveTo) 
          THEN null 
          ELSE CONVERT(Date,MAX(max_EffectiveTo)) 
          END EffectiveTo 
  FROM (
    SELECT ProviderId, CONVERT(varchar,LarsCode) LarsCode, Min(EffectiveFrom) min_EffectiveFrom, Max(EffectiveFrom) max_EffectiveFrom, null max_EffectiveTo
      FROM (
        SELECT ProviderId,LarsCode,[AuditDate] EffectiveFrom
        FROM [dbo].[Audit]
        WHERE UserAction = 'CreateProviderCourse'

        UNION

        SELECT ProviderId, LarsCode, [CreatedDate] EffectiveFrom
        FROM [dbo].[ProviderCourse]
  ) ab1 GROUP BY ProviderId, LarsCode

  UNION ALL
  
  SELECT ProviderId, CONVERT(varchar,LarsCode) LarsCode,null min_EffectiveFrom, null max_EffectiveFrom, MAX(EffectiveTo) max_EffectiveTo
    FROM (
      SELECT ProviderId,LarsCode,null EffectiveFrom,[AuditDate] EffectiveTo
        FROM [dbo].[Audit]
        WHERE UserAction = 'DeleteProviderCourse'
      ) ab2 GROUP BY ProviderId, LarsCode
  ) ab3
  GROUP BY ProviderId, LarsCode
