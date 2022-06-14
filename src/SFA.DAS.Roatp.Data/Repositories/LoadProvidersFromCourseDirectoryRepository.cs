using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class LoadProvidersFromCourseDirectoryRepository : ILoadProvidersFromCourseDirectoryRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<LoadProvidersFromCourseDirectoryRepository> _logger;

        public LoadProvidersFromCourseDirectoryRepository(RoatpDataContext roatpDataContext, ILogger<LoadProvidersFromCourseDirectoryRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> LoadProvidersFromCourseDirectory(List<Provider> providers)
        {
            // await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            // try
            // {
            //     //await _roatpDataContext.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Providers");
            //     //await _roatpDataContext.BulkInsertAsync(providers);
            //     await _roatpDataContext.Providers.AddRangeAsync(providers);
            //     await _roatpDataContext.SaveChangesAsync();
            //     await transaction.CommitAsync();
            // }
            // catch (Exception ex)
            // {
            //     await transaction.RollbackAsync();
            //     _logger.LogError(ex, "Providers load failed on database update");
            //     throw;
            // }

            foreach (var provider in providers)
            {
                try
                {

                    await _roatpDataContext.Providers.AddAsync(provider);
                    await _roatpDataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Providers load failed on database update");
                    continue;
                }
            }

            return true;
        }

    }

}