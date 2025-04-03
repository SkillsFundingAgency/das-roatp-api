using FluentValidation;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryValidator : AbstractValidator<GetCourseProviderDetailsQuery>
{
    public GetCourseProviderDetailsQueryValidator()
    {
        Include(new CoordinatesValidator());
    }
}
