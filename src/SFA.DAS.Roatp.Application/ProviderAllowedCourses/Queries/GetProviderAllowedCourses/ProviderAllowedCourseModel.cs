using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public record ProviderAllowedCourseModel(string LarsCode, string Title, int Level)
{
    public static implicit operator ProviderAllowedCourseModel(ProviderAllowedCourse providerAllowedCourse)
    {
        return new ProviderAllowedCourseModel(providerAllowedCourse.LarsCode, providerAllowedCourse.Standard.Title, providerAllowedCourse.Standard.Level);
    }
}
