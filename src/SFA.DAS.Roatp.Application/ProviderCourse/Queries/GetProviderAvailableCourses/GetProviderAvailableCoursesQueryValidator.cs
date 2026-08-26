using FluentValidation;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;

public class GetProviderAvailableCoursesQueryValidator : AbstractValidator<GetProviderAvailableCoursesQuery>
{
    public GetProviderAvailableCoursesQueryValidator(IValidator<IUkprn> ukprnValidator)
    {
        Include(ukprnValidator);
    }
}
