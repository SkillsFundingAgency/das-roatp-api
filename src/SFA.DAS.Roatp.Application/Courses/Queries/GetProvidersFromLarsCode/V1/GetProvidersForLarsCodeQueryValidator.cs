using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;

public class GetProvidersForLarsCodeQueryValidator : AbstractValidator<GetProvidersForLarsCodeQuery>
{
    public const string OrderByRequiredErrorMessage = "The OrderBy value is required";
    public const string DistanceErrorMessage = "Distance must be greater than 0";

    public GetProvidersForLarsCodeQueryValidator(IStandardsReadRepository standardsReadReadRepository)
    {
        Include(new CoordinatesValidator());
        Include(new LarsCodeValidator(standardsReadReadRepository));
        RuleFor(x => x.OrderBy).NotNull().WithMessage(OrderByRequiredErrorMessage);
        RuleFor(p => p.Distance).GreaterThan(0).WithMessage(DistanceErrorMessage);
        RuleForEach(p => p.DeliveryModes).IsInEnum();
        RuleForEach(p => p.Qar).IsInEnum();
        RuleForEach(p => p.EmployerProviderRatings).IsInEnum();
        RuleForEach(p => p.ApprenticeProviderRatings).IsInEnum();
    }
}