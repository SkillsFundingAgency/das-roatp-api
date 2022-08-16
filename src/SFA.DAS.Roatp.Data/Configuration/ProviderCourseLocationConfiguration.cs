using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    public class ProviderCourseLocationConfiguration : IEntityTypeConfiguration<ProviderCourseLocation>
    {
        public void Configure(EntityTypeBuilder<ProviderCourseLocation> builder)
        {
            builder.ToTable(nameof(ProviderCourseLocation));
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.NavigationId).IsUnique();
            builder.Property(p => p.NavigationId).IsRequired();
            builder.Property(p => p.ProviderCourseId).IsRequired();
            builder.Property(p => p.ProviderLocationId).IsRequired();
            builder.Property(p => p.IsImported).IsRequired();

            builder.HasOne(p => p.Location)
                .WithMany(p => p.ProviderCourseLocations)
                .HasForeignKey(p => p.ProviderLocationId);

            builder.HasOne(p => p.ProviderCourse)
                .WithMany(p => p.Locations)
                .HasForeignKey(p => p.ProviderCourseId);
        }
    }
}
