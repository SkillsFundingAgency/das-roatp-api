using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System.Collections.Generic;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommand : IRequest<ValidatedResponse<int>>, ILarsCodeUkprn, IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public List<int> SelectedSubregionIds { get; set; } = new List<int>();
    }

}
