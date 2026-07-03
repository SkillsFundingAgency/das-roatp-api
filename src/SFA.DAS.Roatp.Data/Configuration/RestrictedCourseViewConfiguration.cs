using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class RestrictedCourseViewConfiguration : IEntityTypeConfiguration<RestrictedCourseView>
{
    public void Configure(EntityTypeBuilder<RestrictedCourseView> builder)
    {
        builder.ToView(nameof(RestrictedCourseView));

        builder.HasKey(x => x.LarsCode);
        builder.Property(x => x.LarsCode).IsRequired();

        builder.HasOne(x => x.Standard)
            .WithOne(x => x.RestrictedCourseView)
            .HasForeignKey<Standard>(x => x.LarsCode)
            .HasPrincipalKey<RestrictedCourseView>(x => x.LarsCode);
    }
}
