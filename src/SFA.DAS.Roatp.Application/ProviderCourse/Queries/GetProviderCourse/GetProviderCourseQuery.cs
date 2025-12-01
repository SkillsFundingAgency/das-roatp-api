using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse
{
    public class GetProviderCourseQuery : IRequest<ValidatedResponse<ProviderCourseModel>>, ILarsCodeUkprn, IUkprn
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetProviderCourseQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
