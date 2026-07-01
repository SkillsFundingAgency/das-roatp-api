using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class RestrictedCourseConfiguration : IEntityTypeConfiguration<RestrictedCourse>
{
    public void Configure(EntityTypeBuilder<RestrictedCourse> builder)
    {
        builder.ToTable(nameof(RestrictedCourse));
        builder.HasMany(x => x.ProviderAllowedCourses)
        .WithOne()
        .HasForeignKey(x => x.LarsCode)
        .HasPrincipalKey(x => x.LarsCode);
    }
}
