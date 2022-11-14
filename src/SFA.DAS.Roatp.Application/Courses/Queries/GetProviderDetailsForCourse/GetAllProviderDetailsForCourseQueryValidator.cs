using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetAllProviderDetailsForCourseQueryValidator : AbstractValidator<GetAllProviderDetailsForCourseQuery>
{
    public GetAllProviderDetailsForCourseQueryValidator(IStandardsReadRepository standardsReadReadRepository)
    {
        Include(new LatLongValidator());
        Include(new LarsCodeValidator(standardsReadReadRepository));
    }
}