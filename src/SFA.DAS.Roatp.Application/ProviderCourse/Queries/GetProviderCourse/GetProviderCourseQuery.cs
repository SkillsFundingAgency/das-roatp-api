using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQuery : IRequest<ValidatedResponse<ProviderCourseModel>>, ILarsCodeUkprn, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetProviderCourseQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
