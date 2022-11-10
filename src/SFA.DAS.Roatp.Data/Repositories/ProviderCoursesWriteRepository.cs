using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCoursesWriteRepository : IProviderCoursesWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCoursesWriteRepository(RoatpDataContext context)
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

        public async Task Delete(int ukprn, int larscode, string userId, string correlationId)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerCourse = await _roatpDataContext.ProviderCourses
                .Where(l => l.LarsCode == larscode && l.Provider.Ukprn == ukprn)
                .Include(l => l.Locations).Include(l => l.Versions)
                .SingleAsync();

                var audit = new Audit
                {
                    //CorrelationId = Guid.Parse(correlationId),
                    CorrelationId = Guid.Parse(Activity.Current?.RootId),
                    EntityType = typeof(ProviderCourse).Name,
                    UserAction = UserActionTypes.DeleteProviderCourse.ToString(),
                    UserId = userId,
                    UserDisplayName = userId, //??
                    EntityId = providerCourse.Id, 
                    InitialState = JsonConvert.SerializeObject(providerCourse,
                            new JsonSerializerSettings
                            {
                                PreserveReferencesHandling = PreserveReferencesHandling.Objects
                            }),
                    AuditDate = DateTime.Now
                };

                _roatpDataContext.Audits.Add(audit);

                _roatpDataContext.Remove(providerCourse);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
