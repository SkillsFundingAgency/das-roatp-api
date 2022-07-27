using MediatR;
using System;

namespace SFA.DAS.Roatp.Application.Location.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQuery : IRequest<GetProviderLocationDetailsQueryResult>
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
