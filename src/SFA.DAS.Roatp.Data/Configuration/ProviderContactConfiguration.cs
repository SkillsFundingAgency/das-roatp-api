using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderContactConfiguration : IEntityTypeConfiguration<ProviderContact>
{
    [ExcludeFromCodeCoverage]
    public void Configure(EntityTypeBuilder<ProviderContact> builder)
    {
        builder.ToTable(nameof(ProviderContact));
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ProviderId).IsRequired();
        builder.Property(p => p.CreatedDate).IsRequired();
        builder.Property(p => p.EmailAddress).HasMaxLength(300);
        builder.Property(p => p.PhoneNumber).HasMaxLength(50);
    }
}