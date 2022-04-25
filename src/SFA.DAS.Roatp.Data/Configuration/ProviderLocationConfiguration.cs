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
            builder.HasIndex(p => p.NavigationId).IsUnique();
            builder.Property(p => p.NavigationId).IsRequired();
            builder.Property(p => p.LocationName).HasMaxLength(250).IsRequired();
            builder.Property(p => p.SubregionName).HasMaxLength(250);
            builder.Property(p => p.AddressLine1).HasMaxLength(250);
            builder.Property(p => p.AddressLine2).HasMaxLength(250);
            builder.Property(p => p.Town).HasMaxLength(250);
            builder.Property(p => p.Postcode).HasMaxLength(25);
            builder.Property(p => p.County).HasMaxLength(250);
            builder.Property(p => p.Latitude).HasColumnType("float").IsRequired();
            builder.Property(p => p.Longitude).HasColumnType("float").IsRequired();
            builder.Property(p => p.Email).HasMaxLength(300);
            builder.Property(p => p.Website).HasMaxLength(500);
            builder.Property(p => p.Phone).HasMaxLength(50);
            builder.Property(p => p.IsImported).IsRequired();

            builder.HasOne(p => p.Provider)
                .WithMany(p => p.Locations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(p => p.ProviderId);
        }
    }

}
