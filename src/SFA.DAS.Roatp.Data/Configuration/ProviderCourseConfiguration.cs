using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderCourseConfiguration : IEntityTypeConfiguration<ProviderCourse>
    {
        [ExcludeFromCodeCoverage]
        public void Configure(EntityTypeBuilder<ProviderCourse> builder)
        {
            builder.ToTable(nameof(ProviderCourse));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProviderId).IsRequired();
            builder.Property(p => p.LarsCode).IsRequired();
            builder.Property(p => p.StandardInfoUrl).HasMaxLength(500);
            builder.Property(p => p.ContactUsEmail).HasMaxLength(500);
            builder.Property(p => p.ContactUsPhoneNumber).HasMaxLength(20);
            builder.Property(p => p.IsImported).IsRequired();

            builder.HasMany(c => c.Locations)
                .WithOne(l => l.ProviderCourse)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(l => l.ProviderCourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Versions)
                .WithOne(l => l.ProviderCourse)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(l => l.ProviderCourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Standard)
                .WithMany(c => c.ProviderCourses)
                .HasPrincipalKey(s => s.LarsCode)
                .HasForeignKey(pc => pc.LarsCode);
        }
    }
}
