using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes
{
    public class GetProviderCourseTypesQueryValidator : AbstractValidator<GetProviderCourseTypesQuery>
    {
        public GetProviderCourseTypesQueryValidator(IProvidersReadRepository providersReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));
        }
    }
}
