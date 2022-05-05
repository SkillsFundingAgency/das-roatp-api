using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.ApiModels.Import
{
    public class CdProvider
    {
        public int Id { get; set; }
        public int Ukprn { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string MarketingInfo { get; set; }
        public decimal? EmployerSatisfaction { get; set; }
        public decimal? LearnerSatisfaction { get; set; }

        public virtual List<CdProviderCourse> Courses { get; set; } = new List<CdProviderCourse>();
    }
}
