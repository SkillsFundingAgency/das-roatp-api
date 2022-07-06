using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommandValidator : AbstractValidator<BulkInsertProviderCourseLocationsCommand>
    {
        public const string EmptptySubregionIdsErrorMessage = "SubregionIds to insert into provider course locations is empty";
        public const string SelectedSubregionIdsNotExistsinProviderLocationsErrorMessage = "Selected SubregionIds are not exists in provider locations";
        public const string SelectedSubregionIdsAlreadyExistsinProviderCourseLocationsErrorMessage = "Selected SubregionIds already exists in provider course locations";
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
