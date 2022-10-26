using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQuery : IRequest<GetAllProviderCoursesQueryResult>, IUkprn
    {
        public int Ukprn { get; }

        public GetAllProviderCoursesQuery(int ukprn) => Ukprn = ukprn;
    }
}