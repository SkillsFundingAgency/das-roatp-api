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
        builder.ToView("RestrictedCourseView")
            .HasNoKey();

        builder.HasOne(x => x.Standard)
        .WithOne()
        .HasForeignKey<RestrictedCourseView>(x => x.LarsCode)
        .HasPrincipalKey<Standard>(x => x.LarsCode);
    }
}
