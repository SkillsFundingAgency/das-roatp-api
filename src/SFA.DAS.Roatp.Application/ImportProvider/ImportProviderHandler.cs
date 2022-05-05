using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Services;
using SFA.DAS.Roatp.Domain.ApiModels.Import;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ImportProvider
{
    public class ImportProviderHandler : IRequestHandler<ImportProviderRequest, bool>
    {
        private readonly IProviderImportRepository _providerImportRepository;
        private readonly IMapProviderService _mapProviderService;
        private readonly ILogger<ImportProviderHandler> _logger;

        public ImportProviderHandler(IProviderImportRepository providerImportRepository, IMapProviderService mapProviderService, ILogger<ImportProviderHandler> logger)
        {
            _providerImportRepository = providerImportRepository;
            _mapProviderService = mapProviderService;
            _logger = logger;
        }

        public async Task<bool> Handle(ImportProviderRequest request, CancellationToken cancellationToken)
        {
            var ukprn = request.Provider.Ukprn;
            var provider = await _mapProviderService.MapProvider(request.Provider);
            if (provider == null)
            {
                _logger.LogWarning("No provider details mapped for Ukprn [{ukprn}]", request.Provider.Ukprn);
                return false;
            }
            _logger.LogInformation("Mapping of provider details completed, starting import for ukprn: {ukprn}",ukprn);
            return await _providerImportRepository.ImportProviderDetails(provider);
        }
    }
}
