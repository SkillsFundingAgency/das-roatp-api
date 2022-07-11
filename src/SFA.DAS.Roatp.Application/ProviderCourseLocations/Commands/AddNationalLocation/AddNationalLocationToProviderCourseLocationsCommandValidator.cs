using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation
{
    public class AddNationalLocationToProviderCourseLocationsCommandValidator : AbstractValidator<AddNationalLocationToProviderCourseLocationsCommand>
    {
        public const string NationalLocationAlreadyExists = "A national location is already associated with this course";
        public AddNationalLocationToProviderCourseLocationsCommandValidator(
            IProviderReadRepository providerReadRepository, 
            IProviderCourseReadRepository providerCourseReadRepository,
            IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(c => c).MustAsync(async (command, cancellationToken) => 
            {
                var allLocations = await providerCourseLocationReadRepository.GetAllProviderCourseLocations(command.Ukprn, command.LarsCode);
                return !allLocations.Any(l => l.Location.LocationType == LocationType.National);
            })
            .WithMessage(NationalLocationAlreadyExists);
        }
    }
}
