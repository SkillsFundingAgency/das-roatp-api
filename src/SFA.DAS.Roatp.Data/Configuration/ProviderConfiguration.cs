using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.ToTable(nameof(Provider));
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.ExternalId).IsUnique();
            builder.Property(p => p.ExternalId).IsRequired();
            builder.Property(p => p.Ukprn).IsRequired();
            builder.Property(p => p.LegalName).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.TradingName).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.Email).HasMaxLength(300);
            builder.Property(p => p.Phone).HasMaxLength(50);
            builder.Property(p => p.Website).HasMaxLength(500);
        }
    }
}
