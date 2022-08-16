using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCourseEditRepository : IProviderCourseEditRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseEditRepository(RoatpDataContext context)
        {
            _roatpDataContext = context;
        }

        public async Task<ProviderCourse> PatchProviderCourse(ProviderCourse patchedProviderCourseEntity)
        {
            var providerCourse = await _roatpDataContext
                .ProviderCourses
                .FindAsync(patchedProviderCourseEntity.Id);

            providerCourse.ContactUsEmail = patchedProviderCourseEntity.ContactUsEmail;
            providerCourse.ContactUsPageUrl = patchedProviderCourseEntity.ContactUsPageUrl;
            providerCourse.ContactUsPhoneNumber = patchedProviderCourseEntity.ContactUsPhoneNumber;
            providerCourse.StandardInfoUrl = patchedProviderCourseEntity.StandardInfoUrl;
            providerCourse.IsApprovedByRegulator = patchedProviderCourseEntity.IsApprovedByRegulator;

            await _roatpDataContext.SaveChangesAsync();

            return providerCourse;
        }

        public async Task<ProviderCourse> CreateProviderCourse(ProviderCourse providerCourse)
        {
            await _roatpDataContext.ProviderCourses.AddAsync(providerCourse);

            await _roatpDataContext.SaveChangesAsync();

            return providerCourse;
        }
    }
}
