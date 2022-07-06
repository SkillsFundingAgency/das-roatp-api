using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandValidator : AbstractValidator<BulkInsertProviderLocationsCommand>
    {
        public const string EmptptySubregionIdsErrorMessage = "Selected SubregionIds to insert into provider locations is empty ";
        public const string RegionsNotFoundErrorMessage = "No Regions data found";
        public const string RegionsAlreadyExistsErrorMessage = "Region is already exists in the provider locations";
        public BulkInsertProviderLocationsCommandValidator(IRegionReadRepository regionReadRepository, IProviderReadRepository providerReadRepository, 
            IProviderCourseReadRepository providerCourseReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(x => x.SelectedSubregionIds)
              .Cascade(CascadeMode.Stop)
              .NotEmpty()
              .WithMessage(EmptptySubregionIdsErrorMessage)
              .MustAsync(async (model, cancellation) =>
              {
                  var regions = await regionReadRepository.GetAllRegions();
                  if (regions == null || !regions.Any()) return false;
                  return true;
              })
              .WithMessage(RegionsNotFoundErrorMessage)
             .MustAsync(async (model, selectedSubregionIds, cancellation) =>
              {
                  var providerLocations = await providerLocationsReadRepository.GetAllProviderLocations(model.Ukprn);

                  return !selectedSubregionIds.Any(a => providerLocations.Exists(b => b.RegionId == a));
              })
              .WithMessage(RegionsAlreadyExistsErrorMessage);
        }
    }
}
