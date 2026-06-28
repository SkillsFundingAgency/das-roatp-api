using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderAllowedCoursesConfiguration : IEntityTypeConfiguration<ProviderAllowedCourse>
{
    public void Configure(EntityTypeBuilder<ProviderAllowedCourse> builder)
    {
        builder.ToTable(nameof(ProviderAllowedCourse));

        builder.HasOne(p => p.Standard)
            .WithMany(s => s.ProviderAllowedCourses)
            .HasPrincipalKey(s => s.LarsCode)
            .HasForeignKey(p => p.LarsCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
