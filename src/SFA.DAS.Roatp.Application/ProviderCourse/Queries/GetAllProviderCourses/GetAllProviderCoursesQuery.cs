using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQuery : IRequest<ValidatedResponse<List<ProviderCourseModel>>>, IUkprn
    {
        public int Ukprn { get; }

        public GetAllProviderCoursesQuery(int ukprn) => Ukprn = ukprn;
    }
}