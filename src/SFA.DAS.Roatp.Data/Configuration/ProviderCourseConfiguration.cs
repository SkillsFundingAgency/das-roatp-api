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
            builder.HasIndex(p => p.ExternalId).IsUnique();
            builder.Property(p => p.ExternalId).IsRequired();
            builder.Property(p => p.ProviderId).IsRequired();
            builder.Property(p => p.LarsCode).IsRequired();
            builder.Property(p => p.IfateReferenceNumber).IsRequired().HasMaxLength(10);
            builder.Property(p => p.StandardInfoUrl).IsRequired().HasMaxLength(500);
            builder.Property(p => p.ContactUsPageUrl).HasMaxLength(500);
            builder.Property(p => p.ContactUsEmail).HasMaxLength(500);
            builder.Property(p => p.ContactUsPhone).HasMaxLength(20);
        }
    }

}
