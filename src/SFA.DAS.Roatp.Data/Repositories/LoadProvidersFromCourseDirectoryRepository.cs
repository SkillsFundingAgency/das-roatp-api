using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
            try
                {
                    await _roatpDataContext.Providers.AddAsync(provider);
                    await _roatpDataContext.SaveChangesAsync();
                }
            //catch (DbUpdateException ex)
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Provider {provider.Ukprn} load failed on database update, message: {ex.Message} : {ex.InnerException?.Message}");

                if (ex?.InnerException?.Message!=null && ex.InnerException.Message.Contains("UK_ProviderLocation_ProviderId_LocationName"))
                {
                    return false;
                }
                if (ex?.InnerException?.Message != null && ex.InnerException.Message.Contains("UK_ProviderCourse_ProviderId_LarsCode"))
                {
                    return false;
                }
                return false;
            }
            
            return true;
        }

    }

}