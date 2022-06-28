﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class LoadProviderFromCourseDirectoryRepository : ILoadProviderFromCourseDirectoryRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<LoadProviderFromCourseDirectoryRepository> _logger;

        public LoadProviderFromCourseDirectoryRepository(RoatpDataContext roatpDataContext, ILogger<LoadProviderFromCourseDirectoryRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> LoadProviderFromCourseDirectory(Provider provider)
        {
            var provs = await _roatpDataContext.Providers.ToListAsync();
            var count = provs.Count;
            var match = provs.Where(x => x.Ukprn == provider.Ukprn);

            if (provider.Ukprn == 10001928)
            {
                var x = "take out the constraint, see what's happening";
            }
            try
            {
                await _roatpDataContext.Providers.AddAsync(provider);
                    await _roatpDataContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Provider {provider.Ukprn} load failed on database update, message: {ex.Message} : {ex.InnerException?.Message}");

                // if (ex?.InnerException?.Message!=null && ex.InnerException.Message.Contains("UK_ProviderLocation_ProviderId_LocationName"))
                // {
                //     _roatpDataContext.Providers.Remove(provider);
                //     return false;
                // }
                // if (ex?.InnerException?.Message != null && ex.InnerException.Message.Contains("UK_ProviderCourse_ProviderId_LarsCode"))
                // {
                //     _roatpDataContext.Providers.Remove(provider);
                //     return false;
                // }

                _roatpDataContext.Providers.Remove(provider);
                return false;
            }
            
            return true;
        }

    }

}