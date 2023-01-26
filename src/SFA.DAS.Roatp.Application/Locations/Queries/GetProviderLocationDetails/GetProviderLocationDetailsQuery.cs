using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQuery : IRequest< ValidatedResponse<ProviderLocationModel>>, IUkprn
    {
        public int Ukprn { get; }

        public Guid Id { get; }

        public GetProviderLocationDetailsQuery(int ukprn, Guid id)
        {
            Ukprn = ukprn;
            Id = id;
        }
    }
}
