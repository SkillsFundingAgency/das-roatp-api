using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommandValidator : AbstractValidator<DeleteProviderCourseLocationCommand>
    {
        public const string InvalidProviderCourseLocationIdErrorMessage = "Invalid Id";
        public const string ProviderCourseLocationNotFoundErrorMessage = "No provider course location found with given ProviderCourseLocationId";
        public DeleteProviderCourseLocationCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository, IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c.Id).
                Cascade(CascadeMode.Stop).
                NotEmpty().WithMessage(InvalidProviderCourseLocationIdErrorMessage)
                .MustAsync(async (model, navigationId, cancellation) =>
                {
                    var providerCourseLocations = await providerCourseLocationReadRepository.GetAllProviderCourseLocations(model.Ukprn, model.LarsCode);
                    return providerCourseLocations.Exists(l=>l.NavigationId == navigationId);
                })
               .WithMessage(ProviderCourseLocationNotFoundErrorMessage);
        }
    }
}
