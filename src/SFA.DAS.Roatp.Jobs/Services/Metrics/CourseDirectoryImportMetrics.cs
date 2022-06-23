namespace SFA.DAS.Roatp.Jobs.Services.Metrics
{
    public class CourseDirectoryImportMetrics
    {
        public int ProvidersToLoad { get; set; }
        public int SuccessfulLoads { get; set; }
        public int FailedMappings { get; set; }
        public int FailedLoads { get; set; }
        public LocationDuplicationMetrics LocationDuplicationMetrics { get; set; }
        public LarsCodeDuplicationMetrics LarsCodeDuplicationMetrics { get; set; }
        public BetaAndPilotProviderMetrics BetaAndPilotProviderMetrics { get; set; }
        public bool BetaAndPilotProvidersOnly { get; set; }
    }
}
