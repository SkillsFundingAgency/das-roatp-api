using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQueryValidator : AbstractValidator<GetProviderCourseQuery>
    {
        public GetProviderCourseQueryValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));
            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));
        }
    }
}
