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
           
            builder.Property(x => x.Address1).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Address2).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Address3).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Address4).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Town).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Postcode).HasMaxLength(25).IsRequired(false);
            builder.Property(x => x.Latitude).IsRequired(false);
            builder.Property(x => x.Latitude).IsRequired(false);
            builder.Property(x => x.AddressUpdateDate).IsRequired();
            builder.Property(x => x.CoordinatesUpdateDate).IsRequired(false);

            builder.HasOne(c => c.Provider)
                       .WithOne(c => c.Address)
                       .HasPrincipalKey<Provider>(c => c.Id)
                       .HasForeignKey<ProviderAddress>(c => c.ProviderId).IsRequired(false)
                       .Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
