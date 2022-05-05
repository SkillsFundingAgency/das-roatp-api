using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Services;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ImportProvider
{
    public class ImportProviderHandler : IRequestHandler<ImportProviderRequest, bool>
    {
        private readonly IImportCourseDetailsRepository _importCourseDetailsRepository;
        private readonly IMapProviderService _mapProviderService;
        private readonly ILogger<ImportProviderHandler> _logger;

        public ImportProviderHandler(IImportCourseDetailsRepository importCourseDetailsRepository, IMapProviderService mapProviderService, ILogger<ImportProviderHandler> logger)
        {
            _importCourseDetailsRepository = importCourseDetailsRepository;
            _mapProviderService = mapProviderService;
            _logger = logger;
        }

        public async Task<bool> Handle(ImportProviderRequest request, CancellationToken cancellationToken)
        {
            var ukprn = request.CdProvider.Ukprn;
            var provider = await _mapProviderService.MapProvider(request.CdProvider);
            _logger.LogInformation("Mapping of provider details completed, starting import for ukprn: {ukprn}",ukprn);
            return await _importCourseDetailsRepository.ImportCourseDetails(provider);
        }
    }
}
