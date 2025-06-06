using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProviderLocationModel = SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations.ProviderLocationModel;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQueryHandler : IRequestHandler<GetProviderLocationDetailsQuery, ValidatedResponse<ProviderLocationModel>>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IStandardsReadRepository _standardsReadRepository;

        public GetProviderLocationDetailsQueryHandler(IProviderLocationsReadRepository providerLocationsReadRepository, IStandardsReadRepository standardsReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _standardsReadRepository = standardsReadRepository;
        }

        public async Task<ValidatedResponse<ProviderLocationModel>> Handle(GetProviderLocationDetailsQuery request, CancellationToken cancellationToken)
        {
            var location = await _providerLocationsReadRepository.GetProviderLocation(request.Ukprn, request.Id);
            var standardsLookup = await _standardsReadRepository.GetAllStandards();

            var result = (ProviderLocationModel)location;

            result.Standards = ProcessStandards(location, standardsLookup);
            return new ValidatedResponse<ProviderLocationModel>(result);
        }

        private static List<LocationStandardModel> ProcessStandards(ProviderLocation location, IReadOnlyCollection<Standard> standardsLookup)
        {
            var standards = new List<LocationStandardModel>();
            foreach (var providerCourseLocation in location.ProviderCourseLocations)
            {
                var standardModel = new LocationStandardModel();
                var matchedStandard = standardsLookup.FirstOrDefault(x => x.LarsCode == providerCourseLocation.ProviderCourse.LarsCode);
                if (matchedStandard == null) continue;
                standardModel.Title = matchedStandard.Title;
                standardModel.LarsCode = matchedStandard.LarsCode;
                standardModel.Level = matchedStandard.Level;

                var providerCourse = location.Provider.Courses.FirstOrDefault(c => c.LarsCode == matchedStandard.LarsCode);

                if (providerCourse != null)
                {
                    standardModel.HasOtherVenues = providerCourse.Locations.Any(l =>
                        l.ProviderLocationId != providerCourseLocation.ProviderLocationId);
                }

                if (standards.All(x => x.LarsCode != matchedStandard.LarsCode))
                {
                    standards.Add(standardModel);
                }
            }

            return standards;
        }
    }
}
