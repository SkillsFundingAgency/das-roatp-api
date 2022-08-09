using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommandValidator : AbstractValidator<AddProviderCourseLocationCommand>
    {
        public const string ProviderCourseOrLocationDetailsErrorMessage = "The provider Course or location details not exists for given ukprn and larscode.";
        public AddProviderCourseLocationCommandValidator
            (IProviderReadRepository providerReadRepository, 
             IProviderCourseReadRepository providerCourseReadRepository, 
             IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c.LocationNavigationId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (command, locationNavigationId, _) => 
                {
                    var providerCourse = await providerCourseReadRepository.GetProviderCourseByUkprn(command.Ukprn, command.LarsCode);
                    var providerLocation = await providerLocationsReadRepository.GetProviderLocation(command.Ukprn, command.LocationNavigationId);
                    return providerCourse != null && providerLocation != null;
                })
                .WithMessage(ProviderCourseOrLocationDetailsErrorMessage);
        }
    }
}
