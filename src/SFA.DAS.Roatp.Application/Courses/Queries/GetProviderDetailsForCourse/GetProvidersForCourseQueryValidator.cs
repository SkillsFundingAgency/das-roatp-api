using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQueryValidator : AbstractValidator<GetProvidersForCourseQuery>
{
    public GetProvidersForCourseQueryValidator(IStandardsReadRepository standardsReadReadRepository)
    {
        Include(new CoordinatesValidator());
        Include(new LarsCodeValidator(standardsReadReadRepository));
    }
}