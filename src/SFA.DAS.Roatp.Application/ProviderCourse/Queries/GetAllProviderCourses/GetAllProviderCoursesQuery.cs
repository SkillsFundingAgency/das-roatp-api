using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
{
    public class GetAllProviderCoursesQuery : IRequest<ValidatedResponse<List<ProviderCourseModel>>>, IUkprn
    {
        public int Ukprn { get; }
        public bool ExcludeInvalidCourses { get; }
        public CourseType? CourseType { get; }
        public GetAllProviderCoursesQuery(int ukprn, bool excludeInvalidCourses, CourseType? courseType)
        {
            Ukprn = ukprn;
            ExcludeInvalidCourses = excludeInvalidCourses;
            CourseType = courseType;
        }
    }
}