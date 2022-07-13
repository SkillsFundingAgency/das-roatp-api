using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries;

namespace SFA.DAS.Roatp.Application.Locations.Queries
{
    public class ProviderAllCoursesQuery : IRequest<ProviderAllCoursesQueryResult>, IUkprn
    {
        public int Ukprn { get; }

        public ProviderAllCoursesQuery(int ukprn) => Ukprn = ukprn;
    }
}