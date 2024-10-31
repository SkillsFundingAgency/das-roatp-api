-- P2-2272 Populate CreatedDate on ProviderCourse from Audit records where available, otherwise default to earliest

MERGE INTO [dbo].[ProviderCourse] pc1
USING
(
	SELECT [Id] ProviderId, LarsCode, AuditDate
	FROM (
		SELECT prv.[Id], JSON_VALUE([initialState],'$.LarsCode') LarsCode, [AuditDate]
		  ,ROW_NUMBER() OVER (PARTITION BY prv.[Id], JSON_VALUE([initialState],'$.LarsCode')ORDER BY [AuditDate] DESC) Seqn
		FROM [dbo].[Audit] aud
		JOIN [dbo].[Provider] prv on prv.Ukprn = Entityid
		WHERE [UserAction] = 'CreateProviderCourse' 
	) ab1 WHERE seqn = 1
) upd
ON pc1.[ProviderId] = upd.[ProviderId] AND pc1.[LarsCode] = upd.LarsCode
WHEN MATCHED THEN
	UPDATE SET pc1.[CreatedDate] = upd.[AuditDate];

UPDATE [dbo].[ProviderCourse]
SET [CreatedDate] = (SELECT DATEADD(day,-1,MIN([CreatedDate])) FROM [dbo].[ProviderCourse])
WHERE [CreatedDate] IS NULL;

