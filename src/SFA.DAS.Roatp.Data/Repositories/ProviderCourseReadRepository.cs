﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCourseReadRepository : IProviderCourseReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseReadRepository(RoatpDataContext roatpDataContext)
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
    }
}
