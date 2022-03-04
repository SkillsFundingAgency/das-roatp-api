using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    public class ProviderLocationConfiguration : IEntityTypeConfiguration<ProviderLocation>
    {
        public void Configure(EntityTypeBuilder<ProviderLocation> builder)
        {
            builder.ToTable(nameof(ProviderLocation));
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.ExternalId).IsUnique();
            builder.Property(p => p.ExternalId).IsRequired();
            builder.Property(p => p.LocationName).HasMaxLength(250);
            builder.Property(p => p.Email).HasMaxLength(300);
            builder.Property(p => p.Website).HasMaxLength(500);
            builder.Property(p => p.Phone).HasMaxLength(20);
            builder.Property(p => p.Website).HasMaxLength(500);
            builder.Property(p => p.AddressLine1).HasMaxLength(250);
            builder.Property(p => p.AddressLine2).HasMaxLength(250);
            builder.Property(p => p.Town).HasMaxLength(250);
            builder.Property(p => p.Postcode).HasMaxLength(25);
            builder.Property(p => p.County).HasMaxLength(250);
        }
    }

}
