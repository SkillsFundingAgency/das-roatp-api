using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;

public class GetAllowedProvidersQueryValidator : AbstractValidator<GetAllowedProvidersQuery>
{
    public GetAllowedProvidersQueryValidator(IStandardsReadRepository standardsReadRepository)
    {
        Include(new LarsCodeValidator(standardsReadRepository));
    }
}
