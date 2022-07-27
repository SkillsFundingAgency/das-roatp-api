using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQuery : IRequest<GetProviderLocationDetailsQueryResult>, IUkprn
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
