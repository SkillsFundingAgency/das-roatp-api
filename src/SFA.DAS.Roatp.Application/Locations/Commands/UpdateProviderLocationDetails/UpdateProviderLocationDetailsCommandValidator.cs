using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using static SFA.DAS.Roatp.Application.Constants;

namespace SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommandValidator : AbstractValidator<UpdateProviderLocationDetailsCommand>
    {
        public const string InvalidIdErrorMessage = "Invalid id";
        public const string ProviderLocationNotFoundErrorMessage = "No provider location found with given ukprn and id";
        public UpdateProviderLocationDetailsCommandValidator(IProviderReadRepository providerReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            RuleFor(x => x.Id)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage(InvalidIdErrorMessage)
               .MustAsync(async (model, id, cancellation) =>
               {
                   var providerLocation = await providerLocationsReadRepository.GetProviderLocation(model.Ukprn, id);
                   return providerLocation != null;
               })
               .WithMessage(ProviderLocationNotFoundErrorMessage);
            RuleFor(c => c.UserId)
                .NotEmpty();
            RuleFor(p => p.LocationName)
                .NotEmpty()
                .MaximumLength(250);
            RuleFor(p => p.Email)
                .MaximumLength(300)
                .Matches(RegularExpressions.EmailRegex);
            RuleFor(p => p.Phone)
                .MinimumLength(10)
                .MaximumLength(50);
            RuleFor(p => p.Website)
                .MaximumLength(500)
                .Matches(RegularExpressions.UrlRegex);
        }
    }
}
