using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommandValidator : AbstractValidator<BulkInsertProviderCourseLocationsCommand>
    {
        public const string EmptptySubregionIdsErrorMessage = "SubregionsIds is required";
        public const string SelectedSubregionIdsNotExistsinProviderLocationsErrorMessage = "Provider locations does not have any or some of the sub-regions being added on the course. It is required to add sub regions to the provider locations before associating them with a course";
        public const string SelectedSubregionIdsAlreadyExistsinProviderCourseLocationsErrorMessage = "All or some of the sub-regions are associated to the provider course. It is required that there are no national or regional locations associated to the course";
        public BulkInsertProviderCourseLocationsCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository, 
            IProviderLocationsReadRepository providerLocationsReadRepository, IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(x => x.SelectedSubregionIds)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage(EmptptySubregionIdsErrorMessage)
              .MustAsync(async (model, ukprn, cancellation) =>
              {
                  var providerLocations = await providerLocationsReadRepository.GetAllProviderLocations(model.Ukprn);
                  return model.SelectedSubregionIds.Any(a => providerLocations.Exists(b => b.RegionId == a));
              })
              .WithMessage(SelectedSubregionIdsNotExistsinProviderLocationsErrorMessage)
             .MustAsync(async (model, ukprn, cancellation) =>
             {
                 var providerCourseLocations = await providerCourseLocationReadRepository.GetAllProviderCourseLocations(model.Ukprn, model.LarsCode);
                 var hasproviderCourseLocations = providerCourseLocations.Any(l => l.Location.LocationType != LocationType.Provider);

                 return  !hasproviderCourseLocations;
             })
              .WithMessage(SelectedSubregionIdsAlreadyExistsinProviderCourseLocationsErrorMessage);
        }
    }
}
