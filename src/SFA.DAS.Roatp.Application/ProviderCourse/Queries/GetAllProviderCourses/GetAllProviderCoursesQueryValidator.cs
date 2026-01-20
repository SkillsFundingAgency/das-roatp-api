using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;

public class GetAllProviderCoursesQueryValidator : AbstractValidator<GetAllProviderCoursesQuery>
{
    public GetAllProviderCoursesQueryValidator(IProvidersReadRepository providersReadRepository)
    {
        Include(new UkprnValidator(providersReadRepository));
    }
}