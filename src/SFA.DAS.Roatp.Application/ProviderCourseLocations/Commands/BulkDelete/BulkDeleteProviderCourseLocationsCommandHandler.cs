using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommandHandler : IRequestHandler<BulkDeleteProviderCourseLocationsCommand, int>
    {
        private readonly IProviderCourseLocationDeleteRepository _providerCourseLocationDeleteRepository;

        public BulkDeleteProviderCourseLocationsCommandHandler(IProviderCourseLocationDeleteRepository providerCourseLocationDeleteRepository)
        {
            _providerCourseLocationDeleteRepository = providerCourseLocationDeleteRepository;
        }

        public async Task<int> Handle(BulkDeleteProviderCourseLocationsCommand request, CancellationToken cancellationToken)
        {
            return await _providerCourseLocationDeleteRepository.BulkDelete(request.Ukprn, request.LarsCode, request.DeleteOptions == DeleteOptions.DeleteProviderLocations);
        }
    }
}
