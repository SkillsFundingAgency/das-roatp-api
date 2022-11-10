using FluentValidation;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetAllProviderDetailsForCourseQueryValidator : AbstractValidator<GetAllProviderDetailsForCourseQuery>
{
    public GetAllProviderDetailsForCourseQueryValidator()
    {
        Include(new LatLongValidator());
    }
}