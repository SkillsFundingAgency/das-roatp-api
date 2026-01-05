using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;


public class GetProviderCourseTypesQueryHandler : IRequestHandler<GetProviderCourseTypesQuery, ValidatedResponse<List<ProviderCourseTypeModel>>>
{
    private readonly IProviderCourseTypesReadRepository _providerCourseTypesReadRepository;

    public GetProviderCourseTypesQueryHandler(IProviderCourseTypesReadRepository providerCourseTypesReadRepository)
    {
        _providerCourseTypesReadRepository = providerCourseTypesReadRepository;
    }

    public async Task<ValidatedResponse<List<ProviderCourseTypeModel>>> Handle(GetProviderCourseTypesQuery request, CancellationToken cancellationToken)
    {
        var providerCourseTypes = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(request.Ukprn);
        var result = providerCourseTypes.Select(x => (ProviderCourseTypeModel)x).ToList();
        return new ValidatedResponse<List<ProviderCourseTypeModel>>(result);
    }
}