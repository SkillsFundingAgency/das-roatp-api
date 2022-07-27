using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAllCourses
{
    public class GetProviderAllCoursesQuery : IRequest<GetProviderAllCoursesQueryResult>, IUkprn
    {
        public int Ukprn { get; }

        public GetProviderAllCoursesQuery(int ukprn) => Ukprn = ukprn;
    }
}