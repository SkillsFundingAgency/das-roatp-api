using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetStandardsService : IGetStandardsService
    {
        private readonly IStandardsRepository _standardsRepository;
        private readonly ILogger<GetStandardsService> _logger;
        public GetStandardsService(IStandardsRepository standardsRepository, ILogger<GetStandardsService> logger)
        {
            _standardsRepository = standardsRepository;
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

            return await _standardsRepository.ReloadStandards(standardsToReload);
        }

        public async Task<List<Standard>> GetStandards()
        {
            var standards = await _standardsRepository.GetStandards();

            return standards.Select(standard => new Standard
                {
                    StandardUid = standard.StandardUId,
                    IfateReferenceNumber = standard.IfateReferenceNumber,
                    LarsCode = standard.LarsCode,
                    Title = standard.Title,
                    Version = standard.Version,
                    Level = standard.Level.ToString()
                })
                .ToList();
        }
    }
}
