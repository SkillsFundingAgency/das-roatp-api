using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommandValidator : AbstractValidator<DeleteProviderCourseLocationCommand>
    {
        public const string InvalidProviderCourseLocationIdErrorMessage = "Invalid larsCode";
        public const string ProviderCourseLocationNotFoundErrorMessage = "No provider course location found with given ProviderCourseLocationId";
        public DeleteProviderCourseLocationCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository, IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c.Id).
                Cascade(CascadeMode.Stop).
                GreaterThan(0).WithMessage(InvalidProviderCourseLocationIdErrorMessage)
                .MustAsync(async (model, providerCourseLocationId, cancellation) =>
                {
                    var providerCourseLocations = await providerCourseLocationReadRepository.GetAllProviderCourseLocations(model.Ukprn, model.LarsCode);
                    return providerCourseLocations.Exists(l=>l.Id == providerCourseLocationId);
                })
               .WithMessage(ProviderCourseLocationNotFoundErrorMessage);
        }
    }
}
