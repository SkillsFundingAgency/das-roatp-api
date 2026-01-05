using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;

public class GetProviderCourseTypesQuery : IRequest<ValidatedResponse<List<ProviderCourseTypeModel>>>, IUkprn
{
    public int Ukprn { get; }

    public GetProviderCourseTypesQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}