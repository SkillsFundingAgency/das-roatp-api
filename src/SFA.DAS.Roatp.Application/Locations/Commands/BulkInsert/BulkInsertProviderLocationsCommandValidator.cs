using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandValidator : AbstractValidator<BulkInsertProviderLocationsCommand>
    {
        public const string RegionsNotFoundErrorMessage = "No Region data found/Region is already exists";
        public BulkInsertProviderLocationsCommandValidator(IRegionReadRepository regionReadRepository, IProviderReadRepository providerReadRepository, 
            IProviderCourseReadRepository providerCourseReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(x => x.Ukprn)
              .Cascade(CascadeMode.Stop)
              .MustAsync(async ( model, ukprn, cancellation) =>
              {
                  var regions = await regionReadRepository.GetAllRegions();
                  if (regions == null || !regions.Any()) return false;
                  var providerLocations = await providerLocationsReadRepository.GetAllProviderLocations(ukprn);
                  foreach (var _ in model.SelectedSubregionIds.Where(Id => providerLocations.Exists(a => a.RegionId == Id)).Select(Id => new { }))
                  {
                      return false;
                  }

                  return true;
              })
              .WithMessage(RegionsNotFoundErrorMessage);
        }
    }
}
