using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderAddressConfiguration : IEntityTypeConfiguration<ProviderAddress>
    {
        public void Configure(EntityTypeBuilder<ProviderAddress> builder)
        {
            builder.ToTable(nameof(ProviderAddress));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProviderId).IsRequired();
           
            builder.Property(x => x.AddressLine1).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.AddressLine2).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.AddressLine3).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.AddressLine4).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Town).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Postcode).HasMaxLength(25).IsRequired(false);
            builder.Property(x => x.Latitude).IsRequired(false);
            builder.Property(x => x.Latitude).IsRequired(false);
            builder.Property(x => x.AddressUpdateDate).IsRequired();
            builder.Property(x => x.CoordinatesUpdateDate).IsRequired(false);
        }
    }
}
