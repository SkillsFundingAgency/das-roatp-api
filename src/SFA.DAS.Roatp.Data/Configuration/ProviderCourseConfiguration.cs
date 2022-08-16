using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    public class ProviderCourseConfiguration : IEntityTypeConfiguration<ProviderCourse>
    {
        public void Configure(EntityTypeBuilder<ProviderCourse> builder)
        {
            builder.ToTable(nameof(ProviderCourse));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProviderId).IsRequired();
            builder.Property(p => p.LarsCode).IsRequired();
            builder.Property(p => p.StandardInfoUrl).HasMaxLength(500);
            builder.Property(p => p.ContactUsPageUrl).HasMaxLength(500);
            builder.Property(p => p.ContactUsEmail).HasMaxLength(500);
            builder.Property(p => p.ContactUsPhoneNumber).HasMaxLength(20);
            builder.Property(p => p.IsImported).IsRequired();

            builder.HasMany(c => c.Locations)
                .WithOne(l => l.ProviderCourse)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(l => l.ProviderCourseId);

            builder.HasMany(c => c.Versions)
                .WithOne(l => l.ProviderCourse)
                .HasPrincipalKey(c => c.Id)
                .HasForeignKey(l => l.ProviderCourseId);
        }
    }
}
