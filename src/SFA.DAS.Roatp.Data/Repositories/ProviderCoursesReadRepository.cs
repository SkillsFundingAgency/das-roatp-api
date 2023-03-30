using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using static System.Int32;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCoursesReadRepository : IProviderCoursesReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCoursesReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderCourse> GetProviderCourse(int providerId, int larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.ProviderId == providerId && c.LarsCode == larsCode)
                .SingleOrDefaultAsync();
        }

        public async Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, int larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.Provider.Ukprn == ukprn && c.LarsCode == larsCode)
                .SingleOrDefaultAsync();
        }

        public async Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.Provider.Ukprn == ukprn)
                .ToListAsync();
        }

        public async Task<int> GetProvidersCount(int larsCode)
        {
            await using var connection = _roatpDataContext.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT count(0)
                    FROM providerCourse PC
                    INNER JOIN Provider P on P.ID = PC.ProviderID
                    LEFT OUTER JOIN ProviderRegistrationDetail PRD on P.ukprn = PRD.ukprn
                    where PC.larsCode ={larsCode}
                    AND PRD.StatusId IN ({OrganisationStatus.Active},{OrganisationStatus.ActiveNotTakingOnApprentices}) 
                    AND PRD.ProviderTypeId={ProviderType.Main}"; 
            var result = await command.ExecuteScalarAsync();
            var success = TryParse(result?.ToString(), out var count);
            if (success)
                return count;

            return 0;
        }
    }
}
