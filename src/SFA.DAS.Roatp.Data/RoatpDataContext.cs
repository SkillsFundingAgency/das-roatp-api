using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data
{
    public class RoatpDataContext : DbContext
    {
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderCourse> ProviderCourses { get; set; }
        public DbSet<ProviderLocation> ProviderLocations { get; set; }
        public DbSet<ProviderCourseLocation> ProviderCoursesLocations { get; set; }
        public DbSet<ProviderCourseVersion> ProviderCoursesVersions { get; set; }

        public RoatpDataContext(DbContextOptions<RoatpDataContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoatpDataContext).Assembly);
        }
    }
}
