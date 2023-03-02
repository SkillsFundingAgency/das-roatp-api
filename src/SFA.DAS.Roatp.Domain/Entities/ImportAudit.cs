using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ImportAudit
    {
        public ImportAudit(DateTime timeStarted, int rowsImported, ImportType importType)
        {
            TimeStarted = timeStarted;
            RowsImported = rowsImported;
            ImportType = importType;
            TimeFinished = DateTime.UtcNow;
        }

        public ImportAudit(DateTime timeStarted, int rowsImported, ImportType importType, DateTime timeFinished)
        {
            TimeStarted = timeStarted;
            RowsImported = rowsImported;
            ImportType = importType;
            TimeFinished = timeFinished;
        }

        public int Id { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeFinished { get; set; }
        public int RowsImported { get; set; }
        public ImportType ImportType { get; set; }
    }

    public enum ImportType
    {
        CourseDirectory,
        ProviderRegistrationDetails,
        ProviderRegistrationAddresses,
        Standards,
        NationalAchievementRates,
        NationalAchievementRatesOverall,
        ProviderAddresses,
        ProviderAddressesLatLong
    }
}
