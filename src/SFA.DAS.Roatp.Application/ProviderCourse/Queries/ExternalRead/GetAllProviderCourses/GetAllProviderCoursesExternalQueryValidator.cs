using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetAllProviderCourses
{
    public class GetAllProviderCoursesExternalQueryValidator : AbstractValidator<GetAllProviderCoursesExternalQuery>
    {
        public GetAllProviderCoursesExternalQueryValidator(IProvidersReadRepository providersReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
        }
    }
}
