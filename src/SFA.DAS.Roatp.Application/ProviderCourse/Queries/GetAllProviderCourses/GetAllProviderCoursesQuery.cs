using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQuery : IRequest<ValidatedResponse<List<ProviderCourseModel>>>, IUkprn
    {
        public int Ukprn { get; }
        public bool ExcludeInvalidCourses { get; }

        public GetAllProviderCoursesQuery(int ukprn, bool excludeInvalidCourses)
        {
            Ukprn = ukprn;
            ExcludeInvalidCourses = excludeInvalidCourses;
        }
    }
}