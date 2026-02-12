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
        public const string ProviderCourses = "Provider Courses";
    }
}
