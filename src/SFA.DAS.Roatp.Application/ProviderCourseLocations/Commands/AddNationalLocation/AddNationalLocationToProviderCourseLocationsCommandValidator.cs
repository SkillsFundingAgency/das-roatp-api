using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Linq;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation
{
    public class AddNationalLocationToProviderCourseLocationsCommandValidator : AbstractValidator<AddNationalLocationToProviderCourseLocationsCommand>
    {
        public const string NationalLocationAlreadyExists = "A national location is already associated with this course";
        public AddNationalLocationToProviderCourseLocationsCommandValidator(
            IProvidersReadRepository providersReadRepository, 
            IProviderCoursesReadRepository providerCoursesReadRepository,
            IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeUkprnCheckerValidator(providersReadRepository, providerCoursesReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c).MustAsync(async (command, cancellationToken) => 
            {
                var allLocations = await providerCourseLocationsReadRepository.GetAllProviderCourseLocations(command.Ukprn, command.LarsCode);
                return !allLocations.Any(l => l.Location.LocationType == LocationType.National);
            })
            .WithMessage(NationalLocationAlreadyExists);
        }
    }
}
