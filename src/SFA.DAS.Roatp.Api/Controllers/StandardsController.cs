using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandardsController : ControllerBase
    {
        private readonly IStandardsReloadRepository _standardsReloadReloadRepository;


        private readonly ILogger<StandardsController> _logger;

        public StandardsController(IStandardsReloadRepository standardsReloadReloadRepository)
        {
            _standardsReloadReloadRepository = standardsReloadReloadRepository;
        }

        [HttpPost]
        [Route("/ReloadStandardsData")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> ReloadStandardsData(StandardsRequest standardRequest)
        { 
            var standardsToProcess = standardRequest.Standards;
            var standardsToReload = standardsToProcess.Select(standard => new Domain.Entities.Standard
                {
                    StandardUId = standard.StandardUid,
                    IfateReferenceNumber = standard.IfateReferenceNumber,
                    LarsCode = standard.LarsCode,
                    Title = standard.Title,
                    Version = standard.Version,
                    Level = Convert.ToInt32(standard.Level)
                })
                .ToList();

            return await _standardsReloadReloadRepository.ReloadStandards(standardsToReload);
        }
    }
}
