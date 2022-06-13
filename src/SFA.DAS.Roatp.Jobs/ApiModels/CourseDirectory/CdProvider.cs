using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory
{
    public class CdProvider
    {
        public int Id { get; set; }
        public int Ukprn { get; set; }
        public string Name { get; set; }

        public string TradingName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string MarketingInfo { get; set; }
        public decimal? EmployerSatisfaction { get; set; }
        public decimal? LearnerSatisfaction { get; set; }

        public virtual List<CdProviderLocation> Locations { get; set; } = new List<CdProviderLocation>();
        public virtual List<CdProviderCourse> Standards { get; set; } = new List<CdProviderCourse>();
       
    }

}
