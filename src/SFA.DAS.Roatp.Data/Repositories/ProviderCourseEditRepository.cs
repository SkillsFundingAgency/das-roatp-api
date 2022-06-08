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

        public async Task UpdateContactDetails(ProviderCourse updatedProviderCourseEntity)
        {
            var providerCourse = await _roatpDataContext
                .ProviderCourses
                .FindAsync(updatedProviderCourseEntity.Id);

            providerCourse.ContactUsEmail = updatedProviderCourseEntity.ContactUsEmail;
            providerCourse.ContactUsPageUrl = updatedProviderCourseEntity.ContactUsPageUrl;
            providerCourse.ContactUsPhoneNumber = updatedProviderCourseEntity.ContactUsPhoneNumber;
            providerCourse.StandardInfoUrl = updatedProviderCourseEntity.StandardInfoUrl;

            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task UpdateConfirmRegulatedStandard(ProviderCourse updatedProviderCourseEntity)
        {
            var providerCourse = await _roatpDataContext
                .ProviderCourses
                .FindAsync(updatedProviderCourseEntity.Id);

            providerCourse.IsApprovedByRegulator = updatedProviderCourseEntity.IsApprovedByRegulator;

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
