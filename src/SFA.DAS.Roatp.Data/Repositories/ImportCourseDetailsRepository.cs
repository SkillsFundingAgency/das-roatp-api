using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ImportCourseDetailsRepository:IImportCourseDetailsRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private ILogger<ImportCourseDetailsRepository> _logger;
        public ImportCourseDetailsRepository(RoatpDataContext roatpDataContext, ILogger<ImportCourseDetailsRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<bool> ImportCourseDetails(Provider provider)
        {
            try
            {
            _roatpDataContext.Providers.Add(provider);
             await _roatpDataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Import provider failed on database insert", ex);
                return false;
            }

            var ukprn = provider.Ukprn;
            _logger.LogInformation("Provider details imported for ukprn {ukprn}", ukprn);
            return true;
        }
    }
}
