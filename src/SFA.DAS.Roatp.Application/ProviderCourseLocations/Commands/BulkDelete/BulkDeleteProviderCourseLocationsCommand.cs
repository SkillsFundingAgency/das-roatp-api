using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommand : IRequest<int>, ILarsCode, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public string UserId { get; }
        public DeleteProviderCourseLocationOption DeleteProviderCourseLocationOptions{ get; }
        public bool DeleteEmployerLocations { get;  }
        public BulkDeleteProviderCourseLocationsCommand(int ukprn, int larsCode, DeleteProviderCourseLocationOption deleteOptions, string userId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            DeleteProviderCourseLocationOptions = deleteOptions;
            UserId = userId;
        }
    }

    public enum DeleteProviderCourseLocationOption
    {
        None,
        DeleteProviderLocations,
        DeleteEmployerLocations
    }
}
