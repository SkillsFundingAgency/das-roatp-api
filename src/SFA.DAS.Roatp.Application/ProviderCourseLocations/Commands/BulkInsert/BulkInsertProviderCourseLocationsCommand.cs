using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommand : IRequest<int>, ILarsCode, IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public List<int> SelectedSubregionIds { get; set; } = new List<int>();
    }

}
