using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetAllProviderCourses
{
    public class GetAllProviderCoursesExternalQuery : IRequest<ValidatedResponse<List<ProviderCourseModelExternal>>>, IUkprn
    {
        public int Ukprn { get; }
        public bool ExcludeInvalidCourses { get; }

        public GetAllProviderCoursesExternalQuery(int ukprn, bool excludeInvalidCourses)
        {
            Ukprn = ukprn;
            ExcludeInvalidCourses = excludeInvalidCourses;
        }
    }
}