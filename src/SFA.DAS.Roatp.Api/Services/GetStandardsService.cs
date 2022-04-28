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
        private readonly IGetStandardsRepository _getStandardsRepository;
        private readonly ILogger<GetStandardsService> _logger;
        public GetStandardsService(IGetStandardsRepository getStandardsRepository, ILogger<GetStandardsService> logger)
        {
            _getStandardsRepository = getStandardsRepository;
            _logger = logger;
        }


        public async Task<List<Standard>> GetStandards()
        {
            var standards = await _getStandardsRepository.GetStandards();
            _logger.LogInformation("Gathering standards");
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
