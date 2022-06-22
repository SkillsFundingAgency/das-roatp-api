namespace SFA.DAS.Roatp.Jobs.Services.Metrics
{
    public class CourseDirectoryImportMetrics
    {
        public int ProvidersToLoad { get; set; }
        public int SuccessfulLoads { get; set; }
        public int FailedMappings { get; set; }
        public int FailedLoads { get; set; }
    }
}
