-- setup the National QAR for the first time

DECLARE 
@counter int,
@TransactionName varchar(20) = 'QAR';

SELECT @counter = COUNT(*) 
FROM [dbo].[NationalQAR] 
WHERE [TimePeriod] = '2223';

IF @counter = 0
BEGIN TRY
BEGIN TRANSACTION @TransactionName;

INSERT INTO [dbo].[NationalQAR] ([Timeperiod], [Leavers],[AchievementRate]) VALUES('2021','275380','57.7');
INSERT INTO [dbo].[NationalQAR] ([Timeperiod], [Leavers],[AchievementRate]) VALUES('2122','263550','53.4');
INSERT INTO [dbo].[NationalQAR] ([Timeperiod], [Leavers],[AchievementRate]) VALUES('2223','293370','54.6');

COMMIT TRANSACTION @TransactionName;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH

