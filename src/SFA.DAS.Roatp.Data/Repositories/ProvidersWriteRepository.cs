﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProvidersWriteRepository : IProvidersWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProvidersWriteRepository> _logger;
        public ProvidersWriteRepository(RoatpDataContext roatpDataContext, ILogger<ProvidersWriteRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task Create(Provider provider)
        {
            _roatpDataContext.Providers.Add(provider);
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task Patch(Provider patchedProviderEntity, string userId, string userDisplayName, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var provider = await _roatpDataContext
               .Providers
               .FindAsync(patchedProviderEntity.Id);

                Audit audit = new(typeof(Provider).Name, patchedProviderEntity.Ukprn.ToString(), userId, userDisplayName, userAction, provider, patchedProviderEntity);

                _roatpDataContext.Audits.Add(audit);

                provider.MarketingInfo = patchedProviderEntity.MarketingInfo;

                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Provider Update is failed for ukprn {ukprn} by userId {userId}", patchedProviderEntity.Ukprn, userId);
                throw;
            }
        }
    }
}
