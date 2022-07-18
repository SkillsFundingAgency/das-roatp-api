using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderCourseQueryValidator : AbstractValidator<ProviderCourseQuery>
    {
        public ProviderCourseQueryValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));
            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));
        }
    }
}
