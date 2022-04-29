using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Api.Requests;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Handlers
{
    public class ReloadStandardsHandler: IRequestHandler<ReloadStandardsRequest,bool>
    {
        private readonly IReloadStandardsRepository _reloadStandardsRepository;
        private readonly ILogger<ReloadStandardsHandler> _logger;

        public ReloadStandardsHandler(IReloadStandardsRepository reloadStandardsRepository, ILogger<ReloadStandardsHandler> logger)
        {
            _reloadStandardsRepository = reloadStandardsRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ReloadStandardsRequest request, CancellationToken cancellationToken)
        {
            var standards = request.Standards;
            if (!standards.Any())
            {
                _logger.LogWarning("No standards were passed into the ReloadStandardsData call, cancelling reload");
                return false;
            
            }
            var standardsToReload = standards.Select(standard => new Domain.Entities.Standard
                {
                    StandardUId = standard.StandardUid,
                    IfateReferenceNumber = standard.IfateReferenceNumber,
                    LarsCode = standard.LarsCode,
                    Title = standard.Title,
                    Version = standard.Version,
                    Level = Convert.ToInt32(standard.Level)
                })
                .ToList();
            
            return await _reloadStandardsRepository.ReloadStandards(standardsToReload);
        }
    }
}
