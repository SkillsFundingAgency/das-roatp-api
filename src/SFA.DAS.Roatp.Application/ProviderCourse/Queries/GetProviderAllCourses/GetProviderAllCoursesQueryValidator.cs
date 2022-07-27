using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAllCourses
{
    public class GetProviderAllCoursesQueryValidator : AbstractValidator<GetProviderAllCoursesQuery>
    {
        public GetProviderAllCoursesQueryValidator(IProviderReadRepository providerReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));
        }
    }
}
