using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandardsController : ControllerBase
    {
        private readonly IReloadStandardsService _reloadStandardsService;


        private readonly ILogger<StandardsController> _logger;

        public StandardsController(IReloadStandardsService reloadStandardsService)
        {
            _reloadStandardsService = reloadStandardsService;
        }

        [HttpPost]
        [Route("/ReloadStandardsData")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> ReloadStandardsData(StandardsRequest standardRequest)
        {
            var standards = standardRequest.Standards;
            return await _reloadStandardsService.ReloadStandards(standards);
        }
    }
}
