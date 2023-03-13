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

        public int Id { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeFinished { get; set; }
        public int RowsImported { get; set; }
        public ImportType ImportType { get; set; }
    }

    public enum ImportType
    {
        ProviderRegistrationDetails,
        ProviderRegistrationAddresses,
        ProviderRegistrationAddressCoordinates,
        Standards,
        NationalAchievementRates,
        NationalAchievementRatesOverall,
        ProviderAddresses,
        ProviderAddressesLatLong
    }
}
