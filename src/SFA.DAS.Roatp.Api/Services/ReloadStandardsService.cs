using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class ReloadStandardsService : IReloadStandardsService
    {
        private readonly IStandardsReloadRepository _standardsReloadReloadRepository;
        private readonly ILogger<ReloadStandardsService> _logger;
        public ReloadStandardsService(IStandardsReloadRepository standardsReloadReloadRepository, ILogger<ReloadStandardsService> logger)
        {
            _standardsReloadReloadRepository = standardsReloadReloadRepository;
            _logger = logger;
        }

        public async Task<bool> ReloadStandards(List<Standard> standards)
        {

            if (standards == null || standards.Count == 0)
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

            var x = await _standardsReloadReloadRepository.ReloadStandards(standardsToReload);
            return true;
        }
    }
}
