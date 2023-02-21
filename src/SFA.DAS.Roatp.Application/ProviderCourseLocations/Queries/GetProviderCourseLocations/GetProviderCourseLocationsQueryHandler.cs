using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQueryHandler : IRequestHandler<GetProviderCourseLocationsQuery, ValidatedResponse<List<ProviderCourseLocationModel>>>
    {
        private readonly IProviderCourseLocationsReadRepository _providerCourseLocationsReadRepository;

        public GetProviderCourseLocationsQueryHandler(IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository)
        {
            _providerCourseLocationsReadRepository = providerCourseLocationsReadRepository;
        }

        public async Task<ValidatedResponse<List<ProviderCourseLocationModel>>> Handle(GetProviderCourseLocationsQuery request, CancellationToken cancellationToken)
        {
            var providerCourseLocations = await _providerCourseLocationsReadRepository.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode);
            var result = providerCourseLocations.Select(x => (ProviderCourseLocationModel)x).ToList();
            

            return new ValidatedResponse<List<ProviderCourseLocationModel>>(result);
        }
    }
}
