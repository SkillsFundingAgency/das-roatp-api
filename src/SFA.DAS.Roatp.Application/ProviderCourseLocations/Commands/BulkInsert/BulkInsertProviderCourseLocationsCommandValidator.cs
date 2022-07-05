using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommandValidator : AbstractValidator<BulkInsertProviderCourseLocationsCommand>
    {
        public const string ProviderDataNotFoundErrorMessage = "relevant provider data not found to insert provider course locations";
        public BulkInsertProviderCourseLocationsCommandValidator(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository, 
            IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            Include(new UkprnValidator(providerReadRepository));

            Include(new LarsCodeValidator(providerReadRepository, providerCourseReadRepository));

            RuleFor(c => c.UserId).NotEmpty();

            RuleFor(x => x.Ukprn)
              .Cascade(CascadeMode.Stop)
             .MustAsync(async (model, ukprn, cancellation) =>
             {
                 var provider = await providerReadRepository.GetByUkprn(ukprn);
                 var providerCourses = await providerCourseReadRepository.GetAllProviderCourses(provider.Id);
                 var providerLocations = await providerLocationsReadRepository.GetAllProviderLocations(ukprn);

                 return model.SelectedSubregionIds.Any(a => providerLocations.Exists(b => b.RegionId == a)) && providerCourses.Any();
             })
              .WithMessage(ProviderDataNotFoundErrorMessage);
        }
    }
}
