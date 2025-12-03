using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse
{
    public class GetProviderCourseExternalQueryValidator : AbstractValidator<GetProviderCourseExternalQuery>
    {
        public GetProviderCourseExternalQueryValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
            Include(new LarsCodeUkprnCombinationValidator(providersReadRepository, providerCoursesReadRepository));
        }
    }
}
