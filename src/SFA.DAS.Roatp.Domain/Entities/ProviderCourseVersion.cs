namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderCourseVersion
    {
        public int ProviderCourseId { get; set; }
        public string StandardUId { get; set; }
        public string Version { get; set; }
        public ProviderCourse ProviderCourse { get; set; }
    }
}
