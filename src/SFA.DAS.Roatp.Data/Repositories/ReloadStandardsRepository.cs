using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class ReloadStandardsRepository : IReloadStandardsRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ReloadStandardsRepository> _logger;

        public ReloadStandardsRepository(RoatpDataContext roatpDataContext, ILogger<ReloadStandardsRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }
        
        public async Task<bool> ReloadStandards(List<Standard> standards)
        {
            var standardsStored = _roatpDataContext.Standards;
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                _roatpDataContext.Standards.RemoveRange(standardsStored);
                _roatpDataContext.Standards.AddRange(standards);
                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Standards reload complete");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Standards reload failed on database update",ex);
                return false;
            }

            return true;
        }
    }
}
