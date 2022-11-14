using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandValidator : AbstractValidator<BulkInsertProviderLocationsCommand>
    {
        public const string EmptptySubregionIdsErrorMessage = "SubregionsIds is required";
        public const string SubRegionsAlreadyExistsErrorMessage = "All or some of the sub-regions already exist on the provider locations";
        public BulkInsertProviderLocationsCommandValidator(IProvidersReadRepository providersReadRepository, 
            IProviderCoursesReadRepository providerCoursesReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providersReadRepository));

            Include(new LarsCodeUkprnCheckerValidator(providersReadRepository, providerCoursesReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(x => x.SelectedSubregionIds)
              .Cascade(CascadeMode.Stop)
              .NotEmpty()
              .WithMessage(EmptptySubregionIdsErrorMessage)
             .MustAsync(async (model, selectedSubregionIds, cancellation) =>
              {
                  var providerLocations = await providerLocationsReadRepository.GetAllProviderLocations(model.Ukprn);

                  return !selectedSubregionIds.Any(a => providerLocations.Exists(b => b.RegionId == a));
              })
              .WithMessage(SubRegionsAlreadyExistsErrorMessage);
        }
    }
}
