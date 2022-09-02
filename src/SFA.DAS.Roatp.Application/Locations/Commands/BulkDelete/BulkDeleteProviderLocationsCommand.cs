using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommand : IRequest<int>,  IUkprn
    {
        public int Ukprn { get; set; }
        public string UserId { get; set; }
        public BulkDeleteProviderLocationsCommand(int ukprn, string userId)
        {
            Ukprn = ukprn;
            UserId = userId;
        }
    }
}
