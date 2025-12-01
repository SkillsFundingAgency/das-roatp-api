using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse
{
    public class GetProviderCourseExternalQuery : IRequest<ValidatedResponse<ProviderCourseModelExternal>>, ILarsCodeUkprn, IUkprn
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetProviderCourseExternalQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
