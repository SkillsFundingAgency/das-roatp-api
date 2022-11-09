using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Roatp.Domain.Models;
using ProviderCourse = SFA.DAS.Roatp.Domain.Entities.ProviderCourse;

namespace SFA.DAS.Roatp.Data
{
    [ExcludeFromCodeCoverage]
    public class RoatpDataContext : DbContext
    {
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderCourse> ProviderCourses { get; set; }
        public DbSet<ProviderLocation> ProviderLocations { get; set; }
        public DbSet<ProviderCourseLocation> ProviderCoursesLocations { get; set; }
        public DbSet<ImportAudit> ImportAudits { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<ProviderCourseVersion> ProviderCoursesVersions { get; set; }
        public DbSet<ProviderRegistrationDetail> ProviderRegistrationDetails { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<NationalAchievementRate> NationalAchievementRates { get; set; }
        public DbSet<NationalAchievementRateImport> NationalAchievementRateImports { get; set; }
        public DbSet<NationalAchievementRateOverall> NationalAchievementRateOverall { get; set; }
        public DbSet<NationalAchievementRateOverallImport> NationalAchievementRateOverallImports { get; set; }
        public DbSet<ProviderAddress> ProviderAddresses { get; set; }

        public DbSet<ProviderCourseDetailsModel> ProviderDetailsWithDistance { get; set; }
        public DbSet<ProviderCourseDetailsSummaryModel> ProviderSummaryDetailsWithDistance { get; set; }

        public DbSet<ProviderCourseLocationDetailsModel> ProviderLocationDetailsWithDistance { get; set; }
        public RoatpDataContext(DbContextOptions<RoatpDataContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoatpDataContext).Assembly);
        }
    }
}
