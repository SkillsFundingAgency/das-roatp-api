using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommand : IRequest<ValidatedResponse<int>>, ILarsCodeUkprn, IUkprn, IUserInfo
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public string UserId { get; }
        public string UserDisplayName { get; }
        public DeleteProviderCourseLocationOption DeleteProviderCourseLocationOptions{ get; }
        public bool DeleteEmployerLocations { get;  }
        public BulkDeleteProviderCourseLocationsCommand(int ukprn, int larsCode, DeleteProviderCourseLocationOption deleteOptions, string userId, string userDisplayName)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            DeleteProviderCourseLocationOptions = deleteOptions;
            UserId = userId;
            UserDisplayName = userDisplayName;
        }
    }

    public enum DeleteProviderCourseLocationOption
    {
        None,
        DeleteProviderLocations,
        DeleteEmployerLocations
    }
}
