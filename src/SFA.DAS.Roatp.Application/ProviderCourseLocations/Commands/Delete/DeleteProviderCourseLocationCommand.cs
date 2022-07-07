using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete
{
    public class DeleteProviderCourseLocationCommand : IRequest<Unit>, ILarsCode, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public int ProviderCourseLocationId { get; set; }
        public string UserId { get; }
        public DeleteProviderCourseLocationCommand(int ukprn, int larsCode, int providerCourseLocationId, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            ProviderCourseLocationId = providerCourseLocationId;
            UserId = userId;
        }
    }
}
