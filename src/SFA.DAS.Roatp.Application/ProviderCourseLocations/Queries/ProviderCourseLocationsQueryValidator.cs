using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQueryValidator : AbstractValidator<ProviderCourseLocationsQuery>
    {
        public const string InvalidUkprnErrorMessage = "Invalid ukprn";
        public const string ProviderNotFoundErrorMessage = "No provider found with given ukprn";
        public const string InvalidLarsCodeErrorMessage = "Invalid larsCode";
        public const string ProviderCourseNotFoundErrorMessage = "No provider course found with given ukprn and larsCode";
        public ProviderCourseLocationsQueryValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository)
        {
            RuleFor(x => x.Ukprn)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(10000000).WithMessage(InvalidUkprnErrorMessage)
                .LessThan(99999999).WithMessage(InvalidUkprnErrorMessage)
                .MustAsync(async (ukprn, cancellation) =>
                {
                    var provider = await providerReadRepository.GetByUkprn(ukprn);
                    return provider != null;
                })
                .WithMessage(ProviderNotFoundErrorMessage);

            RuleFor(x => x.LarsCode)
               .Cascade(CascadeMode.Stop)
               .GreaterThan(0).WithMessage(InvalidLarsCodeErrorMessage)
               .MustAsync(async (model, larsCode, cancellation) =>
               {
                   var provider = await providerReadRepository.GetByUkprn(model.Ukprn);
                   var providerCourse = await providerCourseReadRepository.GetProviderCourse(provider.Id, larsCode);
                   return providerCourse != null;
               })
               .WithMessage(ProviderCourseNotFoundErrorMessage);
        }
    }
}
