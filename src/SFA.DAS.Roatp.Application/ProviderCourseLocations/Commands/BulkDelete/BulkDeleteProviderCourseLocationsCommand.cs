using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommand : IRequest<int>, ILarsCode, IUkprn
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public DeleteOptions DeleteOptions{ get; }
        public bool DeleteEmployerLocations { get;  }
        public BulkDeleteProviderCourseLocationsCommand(int ukprn, int larsCode, DeleteOptions deleteOptions)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            DeleteOptions = deleteOptions;
        }
    }

    public enum DeleteOptions
    {
        None,
        DeleteProviderLocations,
        DeleteEmployerLocations
    }
}
