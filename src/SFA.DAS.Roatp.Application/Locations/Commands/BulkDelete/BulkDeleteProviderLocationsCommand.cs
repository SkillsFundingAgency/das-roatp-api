using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommand : IRequest<int>, ILarsCode, IUkprn
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public BulkDeleteProviderLocationsCommand(int ukprn, int larsCode, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
        }
    }

}
