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
            builder.HasIndex(p => p.ExternalId).IsUnique();
            builder.Property(p => p.ExternalId).IsRequired();
            builder.Property(p => p.ProviderCourseId).IsRequired();
            builder.Property(p => p.ProviderLocationId).IsRequired();
        }
    }

}
