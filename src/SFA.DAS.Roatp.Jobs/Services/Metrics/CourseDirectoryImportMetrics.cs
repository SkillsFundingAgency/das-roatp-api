namespace SFA.DAS.Roatp.Jobs.Services.Metrics
{
    public class CourseDirectoryImportMetrics
    {
        public int TotalProvidersFromCourseDirectory { get; set; }
        public int NumberOfProvidersLoadedSuccessfully { get; set; }
        public int NumberOfProvidersFailedDuringMapping { get; set; }
        public int NumberOfProvidersFailedToLoad { get; set; }
        public LocationDuplicationMetrics LocationDuplicationMetrics { get; set; }
        public LarsCodeDuplicationMetrics LarsCodeDuplicationMetrics { get; set; }
        public PilotProviderMetrics PilotProviderMetrics { get; set; }
        public bool PilotProvidersOnly { get; set; }
        public int TotalStandardsInCache { get; set; }
        public int TotalProvidersOnTheRegister { get; set; }
        public int NumberOfProvidersAlreadyLoaded { get; set; }
        public int TotalNumberOfProvidersToBeLoaded { get; set; }
    }
}
