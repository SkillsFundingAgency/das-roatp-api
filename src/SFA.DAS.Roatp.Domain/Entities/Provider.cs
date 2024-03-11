using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class Provider
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
        public bool IsImported { get; set; } = false;

        public virtual List<ProviderLocation> Locations { get; set; } = new List<ProviderLocation>();

        public virtual List<ProviderCourse> Courses { get; set; } = new List<ProviderCourse>();

        public virtual ProviderAddress ProviderAddress { get; set; }
        public virtual ProviderRegistrationDetail ProviderRegistrationDetail { get; set; }
    }
}
