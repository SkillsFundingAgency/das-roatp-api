using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public static class Constants
{
    public static class EndpointGroups
    {
        public const string Integration = nameof(Integration);
        public const string Management = nameof(Management);
    }

    public static class ApiVersionNumber
    {
        public const string One = "1.0";
        public const string Two = "2.0";
    }

    public static class EndpointTags
    {
        public const string Lookups = nameof(Lookups);
        public const string Standards = nameof(Standards);
        public const string Providers = nameof(Providers);
        public const string ProviderContact = "Provider Contact";
        public const string ProviderCourses = "Provider Courses";
        public const string ProiderCourseLocations = "Provider Course Locations";
        public const string ProviderLocations = "Provider Locations";
        public const string ProviderCoursesTimeLine = "Provider Courses Timeline";
        public const string ProviderAllowedCourses = "Provider Allowed Courses";
    }
}
