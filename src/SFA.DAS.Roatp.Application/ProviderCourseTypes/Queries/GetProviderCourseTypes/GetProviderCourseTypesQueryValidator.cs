using FluentValidation;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes
{
    public class GetProviderCourseTypesQueryValidator : AbstractValidator<GetProviderCourseTypesQuery>
    {
        public GetProviderCourseTypesQueryValidator()
        {
            RuleFor(x => x.Ukprn)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThan(10000000).WithMessage(UkprnValidator.InvalidUkprnErrorMessage)
                .LessThan(19999999).WithMessage(UkprnValidator.InvalidUkprnErrorMessage);
        }
    }
}
