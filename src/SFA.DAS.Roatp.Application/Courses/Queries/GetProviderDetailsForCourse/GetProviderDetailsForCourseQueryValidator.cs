using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse
{
    public class GetProviderDetailsForCourseQueryValidator : AbstractValidator<GetProviderDetailsForCourseQuery>
    {
        public GetProviderDetailsForCourseQueryValidator(IProvidersReadRepository providersReadRepository, IProviderCoursesReadRepository providerCoursesReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
            Include(new LarsCodeUkprnCombinationValidator(providersReadRepository, providerCoursesReadRepository));
            Include(new LatLongValidator());
        }
    }
}
