using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderCoursesTimelineConfiguration : IEntityTypeConfiguration<ProviderCoursesTimeline>
{
    public void Configure(EntityTypeBuilder<ProviderCoursesTimeline> builder)
    {
        builder
            .ToView("ProviderCoursesTimelineView")
            .HasKey(p => new { p.ProviderId, p.LarsCode });

        builder
            .HasOne(p => p.Standard)
            .WithMany(s => s.ProviderCoursesTimelines)
            .HasForeignKey(p => p.LarsCode)
            .HasPrincipalKey(s => s.LarsCode);
    }
}
