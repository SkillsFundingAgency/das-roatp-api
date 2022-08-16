using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse
{
    public class DeleteProviderCourseCommand : IRequest<bool>, ILarsCode, IUkprn
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public DeleteProviderCourseCommand(int ukprn, int larsCode, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
        }
    }
}
